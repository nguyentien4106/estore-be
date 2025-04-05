using TL;

namespace Estore.Application.Services.Telegram.Strategies.DownloadFiles;

public class DownloadImageFileHandler() : IDownloadFileHandler
{
    public InputFileLocationBase GetLocation(TeleFileEntity fileLocation)
    {
        return new InputPhotoFileLocation()
            {
                id = fileLocation.FileId,
                access_hash = fileLocation.AccessHash,
                file_reference = fileLocation.FileReference,
                thumb_size = fileLocation.Thumbnail,
                
            };
    }
}
