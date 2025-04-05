using Estore.Application.Files.Commands.UploadFileTelegram;
using Microsoft.AspNetCore.Http;

namespace Estore.Application.Services.Telegram;

public interface ITelegramService
{
    Task<AppResponse<TeleFileEntity>> UploadFileToStrorageAsync(UploadFileTelegramCommand command, string userId);

    Task<AppResponse<string>> DownloadFileAsync(TeleFileEntity fileLocation);

    Task<AppResponse<bool>> DeleteMessageAsync(int messageId);
}
