using Microsoft.AspNetCore.Identity;

namespace Faice_Backend.Models;

#nullable enable
public class AppUser : IdentityUser
{
    public string? UserRole { get; set; }
    public bool? IsDeleted { get; set; }
}

