using Estore.Application.Constants;
using Estore.Application.Services;

namespace Estore.Application.Store.Commands.StoreImage;

public class DeleteImageHandler(ICloudflareClient client) : ICommandHandler<StoreImageCommand, AppResponse<string>>
{
    public async Task<AppResponse<string>> Handle(StoreImageCommand command, CancellationToken cancellationToken)
    {
        var file = command.File;
        using Stream fileStream = file.OpenReadStream();

        return await client.UploadImageAsync(FileConstants.GetFileName(command.UserName, file.FileName), fileStream, file.ContentType);
    }
}
