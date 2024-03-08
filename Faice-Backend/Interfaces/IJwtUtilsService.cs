using Faice_Backend.Entities;

namespace Faice_Backend.Interfaces;

public interface IJwtUtilsService
{
    public string GenerateJwtToken(User user);
    public int? ValidateJwtToken(string token);
}
