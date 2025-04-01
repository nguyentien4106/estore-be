using Estore.Application.Constants;
using Estore.Application.Services;

namespace Estore.Application.Files.Commands.UploadFile;

public class UploadFileHandler(ICloudflareClient client) : ICommandHandler<UploadFileCommand, AppResponse<R2File>>
{
    public async Task<AppResponse<R2File>> Handle(UploadFileCommand command, CancellationToken cancellationToken)
    {
        var file = command.File;
        using Stream fileStream = file.OpenReadStream();

        return await client.UploadFileAsync(FileConstants.GetFileName(command.UserName, file.FileName), fileStream, file.ContentType);
    }
}
