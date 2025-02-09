namespace Core.Commands.Auth;

/// <summary>
/// Represents a request to refresh a token.
/// </summary>
public class RefreshTokenRequest
{
    /// <summary>
    /// Gets or sets the refresh token.
    /// </summary>
    /// <example>eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...</example>
    public required string RefreshToken { get; set; }

    /// <summary>
    /// Gets or sets the username of the user requesting the token refresh.
    /// </summary>
    /// <example>john.doe</example>
    public required string Username { get; set; }

    /// <summary>
    /// Gets or sets the role of the user requesting the token refresh.
    /// </summary>
    /// <example>User</example>
    public required string Role { get; set; }
}