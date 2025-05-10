using System.Threading.Tasks;

namespace EStore.Application.Services.Webhooks;

public interface IWebhookService : IDisposable
{
    /// <summary>
    /// Sends data to the n8n webhook
    /// </summary>
    /// <typeparam name="T">Type of the data to send</typeparam>
    /// <param name="data">The data to send to the webhook</param>
    /// <returns>Webhook response with success status and data</returns>
    Task<AppResponse<object>> SendToWebhookAsync<T>(T data);
}
