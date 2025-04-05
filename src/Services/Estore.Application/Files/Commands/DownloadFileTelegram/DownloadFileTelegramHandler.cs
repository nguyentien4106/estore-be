using Estore.Application.Helpers;
using Estore.Application.Services;
using Estore.Application.Services.Files;
using Estore.Application.Services.Telegram;

namespace Estore.Application.Files.Commands.DownloadFileTelegram;

public class DownloadFileTelegramHandler(ITelegramService telegramService, IEStoreDbContext context) : ICommandHandler<DownloadFileTelegramCommand, AppResponse<DownloadFileResponse>>
{
    public async Task<AppResponse<DownloadFileResponse>> Handle(DownloadFileTelegramCommand command, CancellationToken cancellationToken)
    {
        var file = await context.TeleFileEntities.FindAsync(command.Id);
        if (file is null)
        {
            return AppResponse<DownloadFileResponse>.NotFound("File", command.Id);
        }

        var result = await telegramService.DownloadFileAsync(file);
        if (result.Succeed)
        {
            return AppResponse<DownloadFileResponse>.Success(new DownloadFileResponse(result.Data, file.ContentType));
        }

        return AppResponse<DownloadFileResponse>.Error(result.Message);
    }
}
