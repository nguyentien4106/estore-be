using TL;

namespace EStore.Application.Services.Telegram.Strategies.DownloadFiles;

public class DownloadDocumentFileHandler : IDownloadFileHandler
{
    public InputFileLocationBase GetLocation(TeleFileEntity fileLocation){
        return new InputDocumentFileLocation()
            {
                id = fileLocation.FileId ?? 0,
                access_hash = fileLocation.AccessHash ?? 0,
                file_reference = fileLocation.FileReference,
            };
    }

}
