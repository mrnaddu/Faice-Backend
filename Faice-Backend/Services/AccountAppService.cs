using Faice_Backend.Data;
using Faice_Backend.Dtos;
using Faice_Backend.Entities;
using Faice_Backend.Helpers;
using Faice_Backend.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Faice_Backend.Services;

public class AccountAppService(
    FaiceDbContext context,
    IJwtUtilsService jwtUtilsService)
        : IAccountAppService
{
    private readonly FaiceDbContext _context = context;
    private readonly IJwtUtilsService _JwtUtilsService = jwtUtilsService;

    public async Task<User> GetById(int id)
    {
        var user = await _context.Users.FirstOrDefaultAsync(x=> x.Id == id);
        return user ?? throw new KeyNotFoundException("User not found");
    }

    public async Task<LoginResponseDto> Login(LoginRequestDto input)
    {
        var user = await _context.Users.SingleOrDefaultAsync(x => x.Username == input.Username);

        // validate
        if (!(user != null && BCrypt.Verify(input.Password, user.PasswordHash)))
            throw new AppException("Username or password is incorrect");

        // authentication successful so generate jwt token
        var jwtToken = _JwtUtilsService.GenerateJwtToken(user);

        return new LoginResponseDto(user, jwtToken);
    }
}
