using Estore.Application.Constants;
using Estore.Application.Services;
using Estore.Application.Files.Commands.DeleteFile;

namespace Estore.Application.Files.Commands.DeleteFile;

public class DeleteFileHandler(ICloudflareClient client) : ICommandHandler<DeleteFileCommand, AppResponse<R2File>>
{
    public async Task<AppResponse<R2File>> Handle(DeleteFileCommand command, CancellationToken cancellationToken)
    {
        return await client.DeleteFileAsync(command.FileName);
    }
}
