namespace Core.Commands.Auth;

public class RefreshTokenRequest
{
    public string Username { get; set; }
    public string Role { get; set; }
    public string RefreshToken { get; set; }
}