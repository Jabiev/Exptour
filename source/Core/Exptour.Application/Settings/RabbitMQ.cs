namespace Exptour.Application.Settings;

public class RabbitMQ
{
    public string HostName { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string VirtualHost { get; set; } = string.Empty;
    public int Port { get; set; }
    public bool UseSSL { get; set; }
}
