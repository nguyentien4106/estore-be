namespace Estore.Application.Services.Files;

public interface IFileHandler
{
    Task<AppResponse<string>> UploadFileAsync(FileHandlerArgs args);
}