
using Estore.Application.Services;

namespace Estore.Application.Store.Commands;

public class StoreImageHandler(ICloudflareClient client) : ICommandHandler<StoreImageCommand, AppResponse<string>>
{
    public async Task<AppResponse<string>> Handle(StoreImageCommand command, CancellationToken cancellationToken)
    {
        using Stream fileStream = command.File.OpenReadStream();

        return await client.UploadImageAsync(command.File.FileName, fileStream, command.File.ContentType);
    }
}
