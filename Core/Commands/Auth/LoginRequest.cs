namespace Core.Commands.Auth;

/// <summary>
/// Represents a request to log in.
/// </summary>
public class LoginRequest
{
    /// <summary>
    /// Gets or sets the username of the user attempting to log in.
    /// </summary>
    /// <example>john.doe</example>
    public required string Username { get; set; }

    /// <summary>
    /// Gets or sets the password of the user attempting to log in.
    /// </summary>
    /// <example>Passw0rd!</example>
    public required string Password { get; set; }

    /// <summary>
    /// Gets or sets the role of the user attempting to log in.
    /// </summary>
    /// <example>User</example>
    public required string Role { get; set; }
}