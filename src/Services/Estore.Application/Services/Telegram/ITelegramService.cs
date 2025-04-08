using EStore.Application.Files.Commands.UploadFileTelegram;
using Microsoft.AspNetCore.Http;

namespace EStore.Application.Services.Telegram;

public interface ITelegramService
{
    Task<AppResponse<TeleFileEntity>> UploadFileToStrorageAsync(UploadFileTelegramCommand command, string userId);

    Task<AppResponse<Stream>> DownloadFileAsync(TeleFileEntity fileLocation);

    Task<AppResponse<bool>> DeleteMessageAsync(int messageId);
}
