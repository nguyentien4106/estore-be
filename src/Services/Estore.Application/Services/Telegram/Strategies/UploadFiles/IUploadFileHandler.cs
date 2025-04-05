using TL;

namespace Estore.Application.Services.Telegram.Strategies.UploadFiles;

public interface IUploadFileHandler
{
    Task<InputMedia> UploadFileAsync(UploadFileHandlerArgs args);
}