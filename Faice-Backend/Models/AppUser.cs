using Microsoft.AspNetCore.Identity;

namespace Faice_Backend.Models;

#nullable enable
public class AppUser : IdentityUser
{
    public string? Name { get; set; }
    public string? PhoneNo { get; set; }
    public string? Password { get; set; }
    public string? UserRole { get; set; }
    public bool? IsDeleted { get; set; }
}

