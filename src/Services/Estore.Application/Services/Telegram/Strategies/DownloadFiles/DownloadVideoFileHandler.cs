using Estore.Application.Helpers;
using TL;

namespace Estore.Application.Services.Telegram.Strategies.DownloadFiles;

public class DownloadVideoFileHandler : IDownloadFileHandler
{
    public InputFileLocationBase GetLocation(TeleFileEntity fileLocation)
    {

        return new InputDocumentFileLocation()
        {
            id = fileLocation.FileId,
            file_reference = fileLocation.FileReference,
            access_hash = fileLocation.AccessHash,
            thumb_size = ""
        };
        var document =  new Document()
        {
            id = fileLocation.FileId,
            access_hash = fileLocation.AccessHash,
            file_reference = fileLocation.FileReference,
        };

        var location = document.ToFileLocation();
        location.thumb_size = "v";

        return location;
    }
}
