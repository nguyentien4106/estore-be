using TL;

namespace EStore.Application.Services.Telegram.Strategies.DownloadFiles;

public class DownloadImageFileHandler() : IDownloadFileHandler
{
    public InputFileLocationBase GetLocation(TeleFileEntity fileLocation)
    {
        return new InputPhotoFileLocation()
            {
                id = fileLocation.FileId ?? 0,
                access_hash = fileLocation.AccessHash ?? 0,
                file_reference = fileLocation.FileReference,
                thumb_size = fileLocation.Thumbnail,
                
            };
    }
}
