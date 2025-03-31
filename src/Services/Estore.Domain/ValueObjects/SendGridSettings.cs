namespace EStore.Domain.ValueObjects;

public class SendGridSettings
{
    public string ApiKey { get; set; } = "";
    public string FromEmail { get; set; } = "";
    public string SenderName { get; set; } = "";
}

