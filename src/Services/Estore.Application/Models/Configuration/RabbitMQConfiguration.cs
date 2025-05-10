namespace EStore.Application.Models.Configuration;

public class RabbitMQConfiguration
{
    public string HostName { get; set; } = "localhost";
    public string? UserName { get; set; }
    public string? Password { get; set; }
    public string VirtualHost { get; set; } = "/";
    public string QueueName { get; set; } = "default-queue"; // Default queue name
}
