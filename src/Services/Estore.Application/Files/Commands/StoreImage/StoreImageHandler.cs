using Estore.Application.Constants;
using Estore.Application.Services;

namespace Estore.Application.Files.Commands.StoreImage;

public class StoreImageHandler(ICloudflareClient client) : ICommandHandler<StoreImageCommand, AppResponse<R2File>>
{
    public async Task<AppResponse<R2File>> Handle(StoreImageCommand command, CancellationToken cancellationToken)
    {
        var file = command.File;
        using Stream fileStream = file.OpenReadStream();

        return await client.UploadImageAsync(FileConstants.GetFileName(command.UserName, file.FileName), fileStream, file.ContentType);
    }
}
