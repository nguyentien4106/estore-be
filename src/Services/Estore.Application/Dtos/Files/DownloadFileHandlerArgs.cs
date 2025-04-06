
namespace EStore.Application.Dtos.Files;

public class DownloadFileHandlerArgs
{
    public WTelegram.Client Client { get; set; }

    public System.IO.Stream FileStream { get; set; }

    public string Caption { get; set; }

    public string FileName { get; set; }

    public int Width { get; set; }

    public int Height { get; set; }
}
