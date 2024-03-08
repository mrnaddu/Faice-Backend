using Faice_Backend.Entities;
using Faice_Backend.Enums;
using Faice_Backend.Helpers;
using Microsoft.AspNetCore.Identity;

namespace Faice_Backend.Data;

public class UsersSeed(ILogger<UsersSeed> logger, UserManager<User> userManager) : IDbSeeder<FaiceDbContext>
{
    public async Task SeedAsync(FaiceDbContext context)
    {
        var admin = await userManager.FindByNameAsync("admin");

        if (admin == null)
        {
            admin = new User
            {
                Id = 1,
                FirstName = "Admin",
                LastName = "User",
                Username = "admin",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin"),
                Role = Role.Admin
            };

            var result = userManager.CreateAsync(admin, BCrypt.Net.BCrypt.HashPassword("admin")).Result;

            if (!result.Succeeded)
            {
                throw new Exception(result.Errors.First().Description);
            }

            if (logger.IsEnabled(LogLevel.Debug))
            {
                logger.LogDebug("admin created");
            }
        }
        else
        {
            if (logger.IsEnabled(LogLevel.Debug))
            {
                logger.LogDebug("admin already exists");
            }
        }
    }
}

