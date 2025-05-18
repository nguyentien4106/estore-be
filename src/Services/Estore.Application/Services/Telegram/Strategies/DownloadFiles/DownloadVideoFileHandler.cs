using EStore.Application.Helpers;
using TL;

namespace EStore.Application.Services.Telegram.Strategies.DownloadFiles;

public class DownloadVideoFileHandler : IDownloadFileHandler
{
    public InputFileLocationBase GetLocation(TeleFileEntity fileLocation)
    {

        return new InputDocumentFileLocation()
        {
            id = fileLocation.FileId ?? 0,
            file_reference = fileLocation.FileReference,
            access_hash = fileLocation.AccessHash ?? 0,
            thumb_size = ""
        };
        var document =  new Document()
        {
            id = fileLocation.FileId ?? 0,
            access_hash = fileLocation.AccessHash ?? 0,
            file_reference = fileLocation.FileReference,
        };

        var location = document.ToFileLocation();
        location.thumb_size = "v";

        return location;
    }
}
