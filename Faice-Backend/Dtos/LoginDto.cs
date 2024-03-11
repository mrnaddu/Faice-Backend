using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Faice_Backend.Dtos;
#nullable enable
public class LoginDto
{
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress]
    public string? Email { get; set; }

    [Required(ErrorMessage = "Password is required")]
    [PasswordPropertyText]
    public string? Password { get; set; }
}
