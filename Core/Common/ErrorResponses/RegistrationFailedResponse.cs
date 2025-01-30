using Microsoft.AspNetCore.Identity;

namespace Core.Common.ErrorResponses;

/// <summary>
/// Represents a response for a failed registration.
/// </summary>
public class RegistrationFailedResponse
{
    /// <summary>
    /// Gets or sets the error message.
    /// </summary>
    /// <example>Username already exists.</example>
    public string Message { get; set; }

    /// <summary>
    /// Creates an identity error response.
    /// </summary>
    /// <param name="result">The identity result containing the errors.</param>
    /// <returns>A <see cref="RegistrationFailedResponse"/> containing the error messages.</returns>
    public static RegistrationFailedResponse IdentityError(IdentityResult result) => new()
        { Message = string.Join(Environment.NewLine, result.Errors.Select(e => e.Description)) };

    /// <summary>
    /// Creates a common error response.
    /// </summary>
    /// <param name="exception">The exception containing the error message.</param>
    /// <returns>A <see cref="RegistrationFailedResponse"/> containing the error message.</returns>
    public static RegistrationFailedResponse CommonError(Exception exception) => new() { Message = exception.Message };
}