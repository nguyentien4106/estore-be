using EStore.Application.Services.Telegram;

namespace EStore.Application.Commands.Files.DownloadFile;

public class DownloadFileTelegramHandler(ITelegramService telegramService, IEStoreDbContext context) : ICommandHandler<DownloadFileTelegramCommand, AppResponse<DownloadFileResult>>
{
    public async Task<AppResponse<DownloadFileResult>> Handle(DownloadFileTelegramCommand command, CancellationToken cancellationToken)
    {
        var file = await context.TeleFileEntities.FindAsync(command.Id, cancellationToken);
        if (file is null)
        {
            return AppResponse<DownloadFileResult>.NotFound("File", command.Id);
        }

        var result = await telegramService.DownloadFileAsync(file);
        
        if (result.Succeed && result.Data is not null)
        {
            return AppResponse<DownloadFileResult>.Success(new DownloadFileResult(result.Data, file.FileName,file.ContentType));
        }

        return AppResponse<DownloadFileResult>.Error(result.Message);
    }
}
