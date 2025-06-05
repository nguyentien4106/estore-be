using EStore.Application.Services.Telegram.Strategies.DownloadFiles;
using EStore.Application.Services.Telegram.Strategies.UploadFiles;

namespace EStore.Application.Services.Telegram.Strategies;

public class TelegramFileHandlerFactory
{
    public static IUploadFileHandler GetUploadFileHandler(FileType fileType)
    {
        return fileType switch
        {
            FileType.Image => new UploadImageFileHandler(),
            FileType.Video => new UploadVideoFileHandler(),
            _ => new UploadDocumentFileHandler(),
        };
    }

    public static IDownloadFileHandler GetDownloadFileHandler(FileType fileType)
    {
        return fileType switch
        {
            FileType.Image => new DownloadImageFileHandler(),
            FileType.Video => new DownloadVideoFileHandler(),
            _ => new DownloadDocumentFileHandler(),
        };
    }
}
