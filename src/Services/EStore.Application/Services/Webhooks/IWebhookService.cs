using System.Threading.Tasks;

namespace EStore.Application.Services.Webhooks;

public interface IWebhookService
{
    /// <summary>
    /// Sends a webhook notification to n8n with the specified event type and payload
    /// </summary>
    /// <param name="eventType">The type of event (e.g., "user.created", "payment.success")</param>
    /// <param name="payload">The data to send in the webhook</param>
    /// <returns>True if the webhook was sent successfully</returns>
    Task<bool> SendWebhookAsync(string eventType, object payload);

    /// <summary>
    /// Sends a webhook notification to n8n with the specified event type, payload, and custom headers
    /// </summary>
    /// <param name="eventType">The type of event</param>
    /// <param name="payload">The data to send in the webhook</param>
    /// <param name="headers">Custom headers to include in the webhook request</param>
    /// <returns>True if the webhook was sent successfully</returns>
    Task<bool> SendWebhookAsync(string eventType, object payload, Dictionary<string, string> headers);

    /// <summary>
    /// Validates if a webhook URL is properly configured for the given event type
    /// </summary>
    /// <param name="eventType">The type of event to check</param>
    /// <returns>True if the webhook is configured for the event type</returns>
    Task<bool> IsWebhookConfiguredAsync(string eventType);
}
