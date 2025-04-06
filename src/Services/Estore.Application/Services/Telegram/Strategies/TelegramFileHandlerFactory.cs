using EStore.Application.Services.Telegram.Strategies.DownloadFiles;
using EStore.Application.Services.Telegram.Strategies.UploadFiles;

namespace EStore.Application.Services.Files;

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

    // public static IDeleteFileHandler GetDeleteFileHandler(StorageSource storageSource, IEStoreDbContext context, ITelegramService telegramService)
    // {
    //     return storageSource switch
    //     {
    //         StorageSource.R2 => new DeleteFileR2Handler(),
    //         StorageSource.Telegram => new DeleteFileTelegramHandler(context, telegramService),
    //         _ => throw new NotSupportedException($"Storage source {storageSource} is not supported")
    //     };
    // }
}
