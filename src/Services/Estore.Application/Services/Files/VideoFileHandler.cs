using Microsoft.AspNetCore.Http;
using TdLib;

namespace Estore.Application.Services.Files;

public class VideoFileHandler : IFileHandler
{

    public Task<AppResponse<string>> UploadFileAsync(FileHandlerArgs args)
    {
        throw new NotImplementedException();
    }
}
