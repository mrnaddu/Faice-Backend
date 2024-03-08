using Faice_Backend.Entities;
using Faice_Backend.Enums;

namespace Faice_Backend.Dtos;

public class LoginResponseDto(User user, string token)
{
    public int Id { get; set; } = user.Id;
    public string FirstName { get; set; } = user.FirstName;
    public string LastName { get; set; } = user.LastName;
    public string Username { get; set; } = user.Username;
    public Role Role { get; set; } = user.Role;
    public string Token { get; set; } = token;
}
