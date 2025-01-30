namespace Core.Common.Options;

/// <summary>
/// Represents the authentication options.
/// </summary>
public class AuthOptions
{
    public static AuthOptions Default { get; } = new()
    {
        AuthServerAddress = "https://localhost:7031/jwks",
        JwtAudience = "https://valid-audience.com",
        JwtIssuer = "https://valid-issuer.com",
        TokenLifetime = TimeSpan.FromSeconds(3600)
    };

    /// <summary>
    /// Gets or sets the authentication server address.
    /// </summary>
    /// <example>https://localhost:7031/jwks</example>
    public string AuthServerAddress { get; set; }

    /// <summary>
    /// Gets or sets the JWT issuer.
    /// </summary>
    /// <example>https://valid-issuer.com</example>
    public string JwtIssuer { get; set; }

    /// <summary>
    /// Gets or sets the JWT audience.
    /// </summary>
    /// <example>https://valid-audience.com</example>
    public string JwtAudience { get; set; }

    /// <summary>
    /// Gets or sets the token lifetime.
    /// </summary>
    /// <example>3600</example>
    public TimeSpan TokenLifetime { get; set; }
}