using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace EStore.Application.Services.Webhooks;

public class N8nWebhookService : IWebhookService
{
    private readonly string _webhookUrl;
    private readonly HttpClient _httpClient;

    public N8nWebhookService(WebhooksConfiguration webhooksConfiguration)   
    {
        _webhookUrl = webhooksConfiguration.N8n.ConfirmEmail;
        _httpClient = new HttpClient();
    }

    /// <summary>
    /// Sends data to the n8n webhook
    /// </summary>
    /// <typeparam name="T">Type of the data to send</typeparam>
    /// <param name="data">The data to send to the webhook</param>
    /// <returns>Webhook response with success status and data</returns>
    public async Task<AppResponse<object>> SendToWebhookAsync<T>(T data)
    {
        try
        {
            var json = JsonSerializer.Serialize(data);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(_webhookUrl, content);
            var responseContent = await response.Content.ReadAsStringAsync();

            return AppResponse<object>.Success(responseContent, "Webhook sent successfully");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error calling n8n webhook: {ex.Message}");
            return AppResponse<object>.Error(ex.Message);
        }
    }

    /// <summary>
    /// Disposes the HttpClient
    /// </summary>
    public void Dispose()
    {
        _httpClient?.Dispose();
    }
} 