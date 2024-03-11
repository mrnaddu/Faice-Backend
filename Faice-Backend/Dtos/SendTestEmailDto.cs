using System.ComponentModel.DataAnnotations;

namespace Faice_Backend.Dtos;
#nullable enable
public class SendTestEmailDto
{
    [EmailAddress]
    [Required(ErrorMessage = "Email is required")]
    public string? Email { get; set; }
    public string? Subject { get; set; }
    public string? Body { get; set; }
}
