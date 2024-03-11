using Faice_Backend.Models;
using Microsoft.AspNetCore.Identity;

namespace Faice_Backend.DataSeeder;

public static class SeedData
{
    public static async Task Initialize(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var services = scope.ServiceProvider;
        var userManager = services.GetRequiredService<UserManager<AppUser>>();
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

        string[] roleNames = [AppRoles.Admin, AppRoles.User, AppRoles.SuperAdmin];
        foreach (var roleName in roleNames)
        {
            var roleExist = await roleManager.RoleExistsAsync(roleName);
            if (!roleExist)
            {
                await roleManager.CreateAsync(new IdentityRole(roleName));
            }
        }

        var adminEmail = "admin@example.com";
        var adminUser = await userManager.FindByEmailAsync(adminEmail);

        if (adminUser == null)
        {
            var newAdminUser = new AppUser
            {
                UserName = "admin",
                Email = adminEmail,
                UserRole = AppRoles.Admin,
                SecurityStamp = Guid.NewGuid().ToString(),
            };

            var result = await userManager.CreateAsync(newAdminUser, "Admin@123");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(newAdminUser, "Admin");
            }
        }

        var superAdminEmail = "superadmin@example.com";
        var superAdmin = await userManager.FindByEmailAsync(superAdminEmail);

        if (superAdmin == null)
        {
            var newSuperAdminUser = new AppUser
            {
                UserName = "superadmin",
                Email = superAdminEmail,
                UserRole= AppRoles.SuperAdmin,
                SecurityStamp = Guid.NewGuid().ToString(),
            };

            var result = await userManager.CreateAsync(newSuperAdminUser, "SuperAdmin@123");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(newSuperAdminUser, "SuperAdmin");
            }
        }
    }
}

