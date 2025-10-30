using System.ComponentModel.DataAnnotations;

namespace Faice_Backend.Dtos;

#nullable enable
public class RefreshTokenDto
{
    [Required]
    public string? AccessToken { get; set; }
    
    [Required]
    public string? RefreshToken { get; set; }
}