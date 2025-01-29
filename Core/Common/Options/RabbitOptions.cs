namespace Core.Common.Options;

public class RabbitOptions
{
    public string Host { get; set; }
    public ushort Port { get; set; } = 5672;
    public string Username { get; set; }
    public string Password { get; set; }
}