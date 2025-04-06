using EStore.Application.Helpers;
using EStore.Application.Services;
using EStore.Application.Services.Cloudflare;
using EStore.Application.Services.Files;
using EStore.Application.Services.Telegram;

namespace EStore.Application.Files.Commands.DownloadFileR2;

public class DownloadFileR2Handler(IEStoreDbContext context, ICloudflareClient r2) : ICommandHandler<DownloadFileR2Command, AppResponse<DownloadFileResponse>>
{
    public async Task<AppResponse<DownloadFileResponse>> Handle(DownloadFileR2Command command, CancellationToken cancellationToken)
    {
        // var deleteFileHandler = TelegramFileHandlerFactory.GetDeleteFileHandler(command.StorageSource, context, telegramService);
        //
        // return await deleteFileHandler.DeleteFileAsync(command.Id, cancellationToken);
        var file = await context.R2FileEntities.FindAsync(command.Id, cancellationToken);
        if (file == null)
        {
            return AppResponse<DownloadFileResponse>.NotFound("File", "Id");
        }

        var url = await r2.GeneratePresignedUrl(file.FileKey);
        
        return AppResponse<DownloadFileResponse>.Success(new (url.Data, file.ContentType));
    }
}
