using Estore.Application.Strategies.DownloadFiles;
using Estore.Application.Strategies.UploadFiles;

namespace Estore.Application.Services.Files;

public class FileHandlerFactory
{
    public static IUploadFileHandler GetUploadFileHandler(FileType fileType)
    {
        return fileType switch
        {
            FileType.Image => new UploadImageFileHandler(),
            FileType.Video => new UploadVideoFileHandler(),
            FileType.Document => new UploadDocumentFileHandler(),
            _ => throw new NotSupportedException($"File type {fileType} is not supported")
        };
    }

    public static IDownloadFileHandler GetDownloadFileHandler(FileType fileType)
    {
        return fileType switch
        {
            FileType.Image => new DownloadImageFileHandler(),
            FileType.Video => new DownloadVideoFileHandler(),
            FileType.Document => new DownloadDocumentFileHandler(),
            _ => throw new NotSupportedException($"File type {fileType} is not supported")
        };
    }
}
