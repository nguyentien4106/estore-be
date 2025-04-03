using Microsoft.AspNetCore.Http;
using TdLib;

namespace Estore.Application.Services.Files;

public class DocumentFileHandler : IFileHandler
{

    public Task<AppResponse<string>> UploadFileAsync(FileHandlerArgs args)
    {
        throw new NotImplementedException();
    }
}
