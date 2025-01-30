namespace Core.Common.ErrorResponses;

/// <summary>
/// Represents a response for an unauthorized request.
/// </summary>
public class UnauthorizedResponse
{
    /// <summary>
    /// Gets or sets the error message.
    /// </summary>
    /// <example>User john.doe is not authorized for role User.</example>
    public string ErrorMessage { get; set; }

    /// <summary>
    /// Creates an unauthorized response.
    /// </summary>
    /// <param name="username">The username of the unauthorized user.</param>
    /// <param name="role">The role the user is unauthorized for.</param>
    /// <returns>A <see cref="UnauthorizedResponse"/> containing the error message.</returns>
    public static UnauthorizedResponse Unauthorized(string username, string role)
    {
        return new UnauthorizedResponse { ErrorMessage = $"User {username} is not authorized for role {role}." };
    }

    /// <summary>
    /// Creates a token not valid response.
    /// </summary>
    /// <param name="token">The invalid token.</param>
    /// <returns>A <see cref="UnauthorizedResponse"/> containing the error message.</returns>
    public static UnauthorizedResponse TokenNotValid(string token)
    {
        return new UnauthorizedResponse { ErrorMessage = $"Token {token} is not valid." };
    }
}