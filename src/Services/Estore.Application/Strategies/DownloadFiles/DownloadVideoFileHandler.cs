using Estore.Application.Helpers;
using TL;

namespace Estore.Application.Strategies.DownloadFiles;

public class DownloadVideoFileHandler : IDownloadFileHandler
{
    public InputFileLocationBase GetLocation(TeleFileLocation fileLocation)
    {
        var document =  new Document()
        {
            id = fileLocation.FileId,
            access_hash = fileLocation.AccessHash,
            file_reference = fileLocation.FileReference,
        };

        return document.ToFileLocation();
    }
}
