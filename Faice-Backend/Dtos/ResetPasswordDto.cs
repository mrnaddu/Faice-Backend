using System.ComponentModel.DataAnnotations;

namespace Faice_Backend.Dtos;

public class ResetPasswordDto
{
    [EmailAddress]
    [Required(ErrorMessage = "Email is required")]
    public string Email { get; set; }
    public string Token { get; set; }
    public string NewPassword { get; set; }
}
