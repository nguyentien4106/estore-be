using TL;

namespace Estore.Application.Strategies.UploadFiles;

public class UploadDocumentFileHandler : IUploadFileHandler
{

    Task<InputMedia> IUploadFileHandler.UploadFileAsync(UploadFileHandlerArgs args)
    {
        throw new NotImplementedException();
    }
}
