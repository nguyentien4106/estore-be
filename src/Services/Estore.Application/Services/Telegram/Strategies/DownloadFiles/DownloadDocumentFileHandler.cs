using TL;

namespace Estore.Application.Services.Telegram.Strategies.DownloadFiles;

public class DownloadDocumentFileHandler : IDownloadFileHandler
{
    public InputFileLocationBase GetLocation(TeleFileEntity fileLocation){
        return new InputDocumentFileLocation()
            {
                id = fileLocation.FileId,
                access_hash = fileLocation.AccessHash,
                file_reference = fileLocation.FileReference,
            };
    }

}
