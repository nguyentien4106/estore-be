using TL;

namespace Estore.Application.Strategies.DownloadFiles;

public class DownloadDocumentFileHandler : IDownloadFileHandler
{
    public InputFileLocationBase GetLocation(TeleFileLocation fileLocation){
        return new InputDocumentFileLocation()
            {
                id = fileLocation.FileId,
                access_hash = fileLocation.AccessHash,
                file_reference = fileLocation.FileReference,
                thumb_size = fileLocation.Thumbnail,
            };
    }

}
