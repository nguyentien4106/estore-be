using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Estore.Application.Services;

public interface ITelegramService
{
    Task SendFileToChannelAsync(string filePath, string caption = null);

    Task<long> GetChatIdAsync();

    Task SendFormFileToChannelAsync(IFormFile file, string caption = null);
}
