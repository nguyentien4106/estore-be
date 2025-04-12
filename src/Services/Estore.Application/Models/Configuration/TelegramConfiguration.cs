namespace EStore.Application.Models.Configuration;

public class TelegramConfiguration
{
    public int ApiId { get; set; }

    public string ApiHash { get; set; }

    public string AppTitle { get; set; }

    public string ShortName { get; set; }

    public string PhoneNumber { get; set; }

    public long ChannelId { get; set; }

    public string AuthCode { get;set; }

    public string TwoFactorPassword { get; set; }

    public string BotToken { get; set; }
}
