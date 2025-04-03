using Microsoft.AspNetCore.Http;

namespace Estore.Application.Services;

public interface ITelegramService
{
    Task<long> GetChatIdAsync();

    Task<AppResponse<long>> UploadFileToStrorageAsync(IFormFile file, string userId);

    Task<AppResponse<bool>> DeleteMessageAsync(long messageId);

    Task<AppResponse<string>> DownloadFileAsync(int messageId);
}
