namespace Core.Commands.Auth;

/// <summary>
/// Represents a request to register a new user.
/// </summary>
public class UserRegistrationRequest
{
    /// <summary>
    /// Gets or sets the username of the new user.
    /// </summary>
    /// <example>john.doe</example>
    public string Username { get; set; }

    /// <summary>
    /// Gets or sets the password of the new user.
    /// </summary>
    /// <example>Passw0rd!</example>
    public string Password { get; set; }
}



/// <summary>
/// Represents a request to register a new courier.
/// </summary>
public class CourierRegistrationRequest
{
    /// <summary>
    /// Gets or sets the username of the new courier.
    /// </summary>
    /// <example>courier.jane</example>
    public string Username { get; set; }

    /// <summary>
    /// Gets or sets the password of the new courier.
    /// </summary>
    /// <example>SecurePass123!</example>
    public string Password { get; set; }
}