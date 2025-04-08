using EStore.Application.Helpers;
using EStore.Application.Services;
using EStore.Application.Services.Files;
using EStore.Application.Services.Telegram;

namespace EStore.Application.Files.Commands.DownloadFileTelegram;

public class DownloadFileTelegramHandler(ITelegramService telegramService, IEStoreDbContext context) : ICommandHandler<DownloadFileTelegramCommand, AppResponse<DownloadFileResponse>>
{
    public async Task<AppResponse<DownloadFileResponse>> Handle(DownloadFileTelegramCommand command, CancellationToken cancellationToken)
    {
        var file = await context.TeleFileEntities.FindAsync(command.Id, cancellationToken);
        if (file is null)
        {
            return AppResponse<DownloadFileResponse>.NotFound("File", command.Id);
        }

        var result = await telegramService.DownloadFileAsync(file);
        
        if (result.Succeed && result.Data is not null)
        {
            return AppResponse<DownloadFileResponse>.Success(new DownloadFileResponse(result.Data, file.FileName,file.ContentType));
        }

        return AppResponse<DownloadFileResponse>.Error(result.Message);
    }
}
