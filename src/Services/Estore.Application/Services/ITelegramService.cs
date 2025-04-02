using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Estore.Application.Services;

public interface ITelegramService
{
    Task<long> GetChatIdAsync();

    Task<AppResponse<long>> UploadFileToStrorageAsync(IFormFile file, string caption = null);

    /// <summary>
    /// Deletes a message from a Telegram channel
    /// </summary>
    /// <param name="messageId">The ID of the message to delete</param>
    /// <param name="revoke">Whether to delete the message for all users</param>
    /// <returns>A task representing the asynchronous operation</returns>
    Task<AppResponse<bool>> DeleteMessageAsync(long messageId);
}
