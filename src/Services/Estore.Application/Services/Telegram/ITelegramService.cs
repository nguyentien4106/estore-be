using Microsoft.AspNetCore.Http;

namespace Estore.Application.Services.Telegram;

public interface ITelegramService
{
    Task<AppResponse<TeleFileLocation>> UploadFileToStrorageAsync(IFormFile file, string userId);

    Task DeleteMessageAsync(long messageId);

    Task<AppResponse<long>> DownloadFileAsync(TeleFileLocation fileLocation);
}
