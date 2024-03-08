using System.ComponentModel.DataAnnotations;

namespace Faice_Backend.Dtos;

public class LoginRequestDto
{
    [Required]
    public string Username { get; set; }

    [Required]
    public string Password { get; set; }
}

