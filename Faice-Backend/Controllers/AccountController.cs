﻿using Faice_Backend.Dtos;
using Faice_Backend.Interfaces;
using Faice_Backend.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
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
    public async Task<IActionResult> Login([FromBody] LoginDto input)
    {
        var user = await _userManager.FindByEmailAsync(input.Email);
        if (user != null && await _userManager.CheckPasswordAsync(user, input.Password))
        {
            var userRoles = await _userManager.GetRolesAsync(user);

            var authClaims = new List<Claim>
            {
                new(ClaimTypes.Name, user.UserName),
                new(ClaimTypes.Email, user.Email),
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };

            foreach (var userRole in userRoles)
            {
                authClaims.Add(new Claim(ClaimTypes.Role, userRole));
            }

            var token = GetToken(authClaims);

            return Ok(new
            {
                token = new JwtSecurityTokenHandler().WriteToken(token),
                expiration = token.ValidTo
            });
        }
        return Unauthorized();
    }

    [HttpGet]
    [Route("logout")]
    public async Task<IActionResult> Logout()
    {
        if (User.Identity.IsAuthenticated)
        {
            await HttpContext.SignOutAsync();

            return Ok(
                new 
                { 
                    message = "Logout successful" 
                });
        }
        else
        {
            return BadRequest(
                new 
                {
                    message = "No active session to log out from" 
                });
        }
    }

    [HttpPost]
    [Route("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto model)
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
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto model)
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
    public async Task<IActionResult> SendResetEmail([FromBody] ForgotPasswordDto model)
    {
        var user = await _userManager.FindByEmailAsync(model.Email);
        if (user == null)
        {
            return NotFound();
        }

        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        var resetLink = $"{Request.Scheme}://{Request.Host}/reset-password?email={user.Email}&token={token}";

        await _emailAppService.SendEmailAsync(user.Email,"Reset Password" ,resetLink);

        return Ok();
    }

    private JwtSecurityToken GetToken(List<Claim> authClaims)
    {
        var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));

        var token = new JwtSecurityToken(
            issuer: _configuration["JWT:ValidIssuer"],
            audience: _configuration["JWT:ValidAudience"],
            expires: DateTime.Now.AddHours(3),
            claims: authClaims,
            signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
            );

        return token;
    }

    /*[HttpGet]
    [Route("current")]
    [Authorize] 
    public IActionResult GetCurrentUserInfo()
    {
        ClaimsPrincipal user = HttpContext.User;

        string userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        string username = user.FindFirst(ClaimTypes.Name)?.Value;

        return Ok(
            new 
            { 
                UserId = userId,
                Username = username 
            });
    }*/
}
