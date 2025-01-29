namespace Core.Common.ErrorResponses;

public class UnauthorizedResponse
{
    public string Message { get; set; }

    public static UnauthorizedResponse Unauthorized(string username, string role) =>
        new() { Message = $"Unauthorized User: {username}, Role: {role}" };

    public static UnauthorizedResponse TokenNotValid(string token) => new() { Message = $"Unauthorized User! Token: {token}" };
}