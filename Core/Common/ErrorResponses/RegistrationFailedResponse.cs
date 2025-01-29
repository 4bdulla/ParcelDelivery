using Microsoft.AspNetCore.Identity;


namespace Core.Common.ErrorResponses;

public class RegistrationFailedResponse
{
    public string Message { get; set; }


    public static RegistrationFailedResponse IdentityError(IdentityResult result) => new()
        { Message = string.Join(Environment.NewLine, result.Errors.Select(e => e.Description)) };

    public static RegistrationFailedResponse CommonError(Exception exception) => new() { Message = exception.Message };
}