namespace Core.Commands.Auth;

public class UserRegistrationRequest
{
    public string Username { get; set; }
    public string Password { get; set; }
}


public class CourierRegistrationRequest
{
    public string Username { get; set; }
    public string Password { get; set; }
    public string Role { get; set; }
}
