using Estore.Application.Files.Commands.UploadFileTelegram;
using Microsoft.AspNetCore.Http;

namespace Estore.Application.Services.Telegram;

public interface ITelegramService
{
    Task<AppResponse<TeleFileLocation>> UploadFileToStrorageAsync(UploadFileTelegramCommand command, string userId);

    Task<AppResponse<string>> DownloadFileAsync(TeleFileLocation fileLocation);

    Task DeleteMessageAsync(long messageId);
}
