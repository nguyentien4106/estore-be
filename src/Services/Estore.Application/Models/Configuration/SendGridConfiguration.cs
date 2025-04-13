
namespace EStore.Application.Models.Configuration;

public class SendGridConfiguration
{
    public string ApiKey { get; set; } = "";
    public string FromEmail { get; set; } = "";
    public string SenderName { get; set; } = "";
}

