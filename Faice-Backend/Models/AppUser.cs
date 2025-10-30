using Microsoft.AspNetCore.Identity;

namespace Faice_Backend.Models;

#nullable enable
public class AppUser : IdentityUser
{
    public Guid Id { get; set; }
    public string? UserRole { get; set; }
    public bool? IsDeleted { get; set; }
    public string? RefreshToken { get; set; }
    public DateTime RefreshTokenExpiryTime { get; set; }
}

