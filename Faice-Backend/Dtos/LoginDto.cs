using System.ComponentModel.DataAnnotations;

namespace Faice_Backend.Dtos;
#nullable enable
public class LoginDto
{
    [Required(ErrorMessage = "User Name is required")]
    public string? Username { get; set; }

    [Required(ErrorMessage = "Password is required")]
    public string? Password { get; set; }
}
