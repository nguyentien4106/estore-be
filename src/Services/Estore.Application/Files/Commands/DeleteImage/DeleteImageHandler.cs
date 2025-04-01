using Estore.Application.Constants;
using Estore.Application.Services;
using Estore.Application.Files.Commands.DeleteImage;

namespace Estore.Application.Files.Commands.DeleteImage;

public class DeleteImageHandler(ICloudflareClient client) : ICommandHandler<DeleteImageCommand, AppResponse<R2File>>
{
    public async Task<AppResponse<R2File>> Handle(DeleteImageCommand command, CancellationToken cancellationToken)
    {
        return await client.DeleteImageAsync(command.FileName);
    }
}
