using TL;

namespace Estore.Application.Strategies.UploadFiles;

public interface IUploadFileHandler
{
    Task<InputMedia> UploadFileAsync(UploadFileHandlerArgs args);
}