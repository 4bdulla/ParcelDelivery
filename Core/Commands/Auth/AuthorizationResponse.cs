namespace Core.Commands.Auth;

public class AuthorizationResponse
{
    public int UserId { get; set; }
    public string AccessToken { get; set; }
    public string RefreshToken { get; set; }
}