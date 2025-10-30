using Faice_Backend.Dtos;
using Faice_Backend.Interfaces;
using Faice_Backend.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Faice_Backend.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AccountController(
    UserManager<AppUser> userManager,
    IConfiguration configuration,
    IEmailAppService emailAppService) : ControllerBase
{
    private readonly UserManager<AppUser> _userManager = userManager;
    private readonly IConfiguration _configuration = configuration;
    private readonly IEmailAppService _emailAppService = emailAppService;

    [HttpPost]
    [Route("login")]
    public async Task<IActionResult> LoginAsync([FromBody] LoginDto input)
    {
        var user = await _userManager.FindByEmailAsync(input.Email);
        if (user != null && await _userManager.CheckPasswordAsync(user, input.Password))
        {
            var userRoles = await _userManager.GetRolesAsync(user);
            var authClaims = CreateClaims(user, userRoles);

            var accessToken = GenerateAccessToken(authClaims);
            var idToken = GenerateIdToken(user, userRoles);
            var refreshToken = GenerateRefreshToken();
            var refreshTokenExpiry = DateTime.UtcNow.AddDays(7);

            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = refreshTokenExpiry;
            await _userManager.UpdateAsync(user);

            return Ok(new TokenResponseDto
            {
                AccessToken = new JwtSecurityTokenHandler().WriteToken(accessToken),
                IdToken = new JwtSecurityTokenHandler().WriteToken(idToken),
                RefreshToken = refreshToken,
                AccessTokenExpiration = accessToken.ValidTo,
                IdTokenExpiration = idToken.ValidTo,
                RefreshTokenExpiration = refreshTokenExpiry
            });
        }
        return Unauthorized(new { message = "Invalid credentials" });
    }

    [HttpPost]
    [Route("logout")]
    [Authorize]
    public async Task<IActionResult> LogoutAsync()
    {
        var email = User.FindFirst(ClaimTypes.Email)?.Value;
        if (!string.IsNullOrEmpty(email))
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user != null)
            {
                user.RefreshToken = null;
                user.RefreshTokenExpiryTime = DateTime.UtcNow;
                await _userManager.UpdateAsync(user);
            }
        }

        await HttpContext.SignOutAsync();
        return Ok(new { message = "LogoutAsync successful" });
    }

    [HttpPost]
    [Route("register")]
    public async Task<IActionResult> RegisterAsync([FromBody] RegisterDto model)
    {
        var userExists = await _userManager.FindByNameAsync(model.Username);
        if (userExists != null)
            return StatusCode(StatusCodes.Status500InternalServerError, new ResponseDto { Status = "Error", Message = "User already exists!" });

        AppUser user = new()
        {
            Email = model.Email,
            SecurityStamp = Guid.NewGuid().ToString(),
            UserName = model.Username,
            EmailConfirmed = false,
            UserRole = AppRoles.User,
            PhoneNumber = model.PhoneNumber,
            PhoneNumberConfirmed = false,
            TwoFactorEnabled = false,
        };

        var result = await _userManager.CreateAsync(user, model.Password);
        await _userManager.AddToRoleAsync(user, AppRoles.User);
        if (!result.Succeeded)
            return StatusCode(StatusCodes.Status500InternalServerError,
                new ResponseDto
                {
                    Status = "Error",
                    Message = "User creation failed! Please check user details and try again."
                });

        return Ok(new ResponseDto
        {
            Status = "Success",
            Message = "User created successfully!"
        });
    }


    [HttpPost]
    [Route("reset-password")]
    public async Task<IActionResult> ResetPasswordAsync([FromBody] ResetPasswordDto model)
    {
        var user = await _userManager.FindByEmailAsync(model.Email);
        if (user == null)
        {
            return NotFound();
        }

        var result = await _userManager.ResetPasswordAsync(user, model.Token, model.NewPassword);
        if (result.Succeeded)
        {
            return Ok(
                new
                {
                    message = "Password reset successful"
                });
        }
        else
        {
            return BadRequest(
                new
                {
                    message = "Password reset failed"
                });
        }
    }

    [HttpPost]
    [Route("send-reset-email")]
    public async Task<IActionResult> SendResetEmailAsync([FromBody] ForgotPasswordDto model)
    {
        var user = await _userManager.FindByEmailAsync(model.Email);
        if (user == null)
        {
            return NotFound();
        }

        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        var resetLink = $"{Request.Scheme}://{Request.Host}/reset-password?email={user.Email}&token={token}";

        await _emailAppService.SendEmailAsync(user.Email, "Reset Password", resetLink);

        return Ok();
    }

    [HttpPost]
    [Route("refresh-token")]
    public async Task<IActionResult> RefreshTokenAsync([FromBody] RefreshTokenDto tokenDto)
    {
        var principal = GetPrincipalFromExpiredToken(tokenDto.AccessToken);
        if (principal?.Identity?.Name == null)
            return BadRequest(new { message = "Invalid access token" });

        var user = await _userManager.FindByNameAsync(principal.Identity.Name);
        if (user == null || user.RefreshToken != tokenDto.RefreshToken || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
            return BadRequest(new { message = "Invalid refresh token" });

        var userRoles = await _userManager.GetRolesAsync(user);
        var authClaims = CreateClaims(user, userRoles);
        var newAccessToken = GenerateAccessToken(authClaims);
        var newIdToken = GenerateIdToken(user, userRoles);
        var newRefreshToken = GenerateRefreshToken();
        var refreshTokenExpiry = DateTime.UtcNow.AddDays(7);

        user.RefreshToken = newRefreshToken;
        user.RefreshTokenExpiryTime = refreshTokenExpiry;
        await _userManager.UpdateAsync(user);

        return Ok(new TokenResponseDto
        {
            AccessToken = new JwtSecurityTokenHandler().WriteToken(newAccessToken),
            IdToken = new JwtSecurityTokenHandler().WriteToken(newIdToken),
            RefreshToken = newRefreshToken,
            AccessTokenExpiration = newAccessToken.ValidTo,
            IdTokenExpiration = newIdToken.ValidTo,
            RefreshTokenExpiration = refreshTokenExpiry
        });
    }

    [HttpGet]
    [Route("current")]
    [Authorize]
    public Task<IActionResult> GetCurrentUserInfo()
    {
        var email = User.FindFirst(ClaimTypes.Email)?.Value;
        var username = User.FindFirst(ClaimTypes.Name)?.Value;
        var userId = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
        var roles = User.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList();

        return Task.FromResult<IActionResult>(Ok(new
        {
            UserId = userId,
            Username = username,
            Email = email,
            Roles = roles
        }));
    }

    private static List<Claim> CreateClaims(AppUser user, IList<string> userRoles)
    {
        var authClaims = new List<Claim>
        {
            new(ClaimTypes.Name, user.UserName ?? string.Empty),
            new(ClaimTypes.Email, user.Email ?? string.Empty),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString())
        };

        foreach (var userRole in userRoles)
        {
            authClaims.Add(new Claim(ClaimTypes.Role, userRole));
        }

        return authClaims;
    }

    private JwtSecurityToken GenerateAccessToken(List<Claim> authClaims)
    {
        var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"] ?? throw new InvalidOperationException("JWT Secret not configured")));

        return new JwtSecurityToken(
            issuer: _configuration["JWT:ValidIssuer"],
            audience: _configuration["JWT:ValidAudience"],
            expires: DateTime.UtcNow.AddMinutes(15),
            claims: authClaims,
            signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
        );
    }

    private JwtSecurityToken GenerateIdToken(AppUser user, IList<string> userRoles)
    {
        var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"] ?? throw new InvalidOperationException("JWT Secret not configured")));

        var idClaims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(JwtRegisteredClaimNames.Email, user.Email ?? string.Empty),
            new(JwtRegisteredClaimNames.Name, user.UserName ?? string.Empty),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64),
            new("phone_number", user.PhoneNumber ?? string.Empty),
            new("email_verified", user.EmailConfirmed.ToString().ToLower()),
            new("phone_number_verified", user.PhoneNumberConfirmed.ToString().ToLower())
        };

        foreach (var role in userRoles)
        {
            idClaims.Add(new Claim("roles", role));
        }

        return new JwtSecurityToken(
            issuer: _configuration["JWT:ValidIssuer"],
            audience: _configuration["JWT:ValidAudience"],
            expires: DateTime.UtcNow.AddHours(1),
            claims: idClaims,
            signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
        );
    }

    private static string GenerateRefreshToken()
    {
        var randomNumber = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }

    private ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
    {
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = false,
            ValidateIssuer = false,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"] ?? throw new InvalidOperationException("JWT Secret not configured"))),
            ValidateLifetime = false
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);

        if (securityToken is not JwtSecurityToken jwtSecurityToken ||
            !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            throw new SecurityTokenException("Invalid token");

        return principal;
    }
}
