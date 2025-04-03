namespace Estore.Application.Services.Files;

public class FileHandlerFactory
{
    public static IFileHandler GetHandler(FileType fileType, TelegramConfiguration config)
    {
        return fileType switch
        {
            FileType.Image => new ImageFileHandler(config),
            FileType.Video => new VideoFileHandler(),
            FileType.Document => new DocumentFileHandler(),
            _ => throw new NotSupportedException($"File type {fileType} is not supported")
        };
    }
}
