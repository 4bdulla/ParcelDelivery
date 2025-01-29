namespace Core.Common.Options;

public class AuthOptions
{
    public static AuthOptions Default { get; } = new()
    {
        AuthServerAddress = "https://localhost:7031/jwks",
        JwtAudience = "https://valid-audience.com",
        JwtIssuer = "https://valid-issuer.com",
        TokenLifetime = TimeSpan.FromSeconds(3600)
    };

    public string AuthServerAddress { get; set; }
    public string JwtIssuer { get; set; }
    public string JwtAudience { get; set; }
    public TimeSpan TokenLifetime { get; set; }
}