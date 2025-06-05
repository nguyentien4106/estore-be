
using TL;
namespace EStore.Application.Models.Files;

public class UploadFileHandlerArgs
{
    public WTelegram.Client? Client { get; set; }

    public Stream? FileStream { get; set; }

    public string? Caption { get; set; } = string.Empty;

    public string? FileName { get; set; } = string.Empty;

    public string? ContentType { get; set; } = string.Empty;

    public long ContentLength { get; set; } 

    public string? FilePath { get; set; } = string.Empty;

    public long ChannelId { get; set; }

    public required Guid FileId { get; set; }

    public WTelegram.Client.ProgressCallback? ProgressCallback { get; set; }

}
