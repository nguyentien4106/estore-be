
namespace EStore.Application.Models.Files;

public class UploadFileHandlerArgs
{
    public WTelegram.Client Client { get; set; }

    public Stream FileStream { get; set; }

    public string Caption { get; set; }

    public string FileName { get; set; }

    public string ContentType { get; set; }

    public long ContentLength { get; set; } 
}
