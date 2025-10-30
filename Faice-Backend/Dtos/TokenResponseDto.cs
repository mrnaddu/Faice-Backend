namespace Faice_Backend.Dtos;

#nullable enable
public class TokenResponseDto
{
    public string? AccessToken { get; set; }
    public string? IdToken { get; set; }
    public string? RefreshToken { get; set; }
    public DateTime AccessTokenExpiration { get; set; }
    public DateTime IdTokenExpiration { get; set; }
    public DateTime RefreshTokenExpiration { get; set; }
    public string? TokenType { get; set; } = "Bearer";
}