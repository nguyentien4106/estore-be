using EStore.Application.Services.Cloudflare;

namespace EStore.Application.Commands.Files.DownloadFile;

public class DownloadFileR2Handler(IEStoreDbContext context, ICloudflareClient r2) : ICommandHandler<DownloadFileR2Command, AppResponse<DownloadFileResult>>
{
    public async Task<AppResponse<DownloadFileResult>> Handle(DownloadFileR2Command command, CancellationToken cancellationToken)
    {
        var file = await context.R2FileEntities.FindAsync(command.Id, cancellationToken);
        if (file == null)
        {
            return AppResponse<DownloadFileResult>.NotFound("File", "Id");
        }

        var stream = await r2.DownloadFile(file.FileKey);
        
        if (stream.Succeed && stream.Data is not null)
        {
            return AppResponse<DownloadFileResult>.Success(new (stream.Data, file.FileName, file.ContentType));
        }

        return AppResponse<DownloadFileResult>.Error(stream.Message);
    }
}
