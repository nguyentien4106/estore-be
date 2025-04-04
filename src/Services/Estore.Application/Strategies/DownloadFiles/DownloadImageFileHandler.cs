using TL;

namespace Estore.Application.Strategies.DownloadFiles;

public class DownloadImageFileHandler() : IDownloadFileHandler
{
    public InputFileLocationBase GetLocation(TeleFileLocation fileLocation)
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
