using EStore.Application.Services.Cloudflare;

namespace EStore.Application.Commands.Files.DeleteFile;

public class DeleteFileHandler(ICloudflareClient r2, IEStoreDbContext context) : ICommandHandler<DeleteFileR2Command, AppResponse<Guid>>
{
    public async Task<AppResponse<Guid>> Handle(DeleteFileR2Command command, CancellationToken cancellationToken)
    {
        var file = await context.R2FileEntities.FindAsync(command.Id, cancellationToken);
        if (file == null)
        {
            return AppResponse<Guid>.NotFound("File", "Id");
        }

        var result = await r2.DeleteFileAsync(file.FileKey);
        if(!result.Succeed){
            return AppResponse<Guid>.Error(result.Message);
        };

        return AppResponse<Guid>.Success(command.Id);
    }
}
