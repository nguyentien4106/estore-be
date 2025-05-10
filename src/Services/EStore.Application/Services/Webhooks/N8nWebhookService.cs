using System.Net.Http.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace EStore.Application.Services.Webhooks;

public class N8nWebhookService : IWebhookService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;
    private readonly ILogger<N8nWebhookService> _logger;
    private readonly Dictionary<string, string> _webhookUrls;

    public N8nWebhookService(
        IHttpClientFactory httpClientFactory,
        IConfiguration configuration,
        ILogger<N8nWebhookService> logger)
    {
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
        _logger = logger;
        _webhookUrls = LoadWebhookUrls();
    }

    public async Task<bool> SendWebhookAsync(string eventType, object payload)
    {
        return await SendWebhookAsync(eventType, payload, new Dictionary<string, string>());
    }

    public async Task<bool> SendWebhookAsync(string eventType, object payload, Dictionary<string, string> headers)
    {
        try
        {
            if (!await IsWebhookConfiguredAsync(eventType))
            {
                _logger.LogWarning("No webhook URL configured for event type: {EventType}", eventType);
                return false;
            }

            var webhookUrl = _webhookUrls[eventType];
            var client = _httpClientFactory.CreateClient("n8n-webhook");

            // Add default headers
            client.DefaultRequestHeaders.Add("X-Event-Type", eventType);
            client.DefaultRequestHeaders.Add("X-Webhook-Source", "estore-api");

            // Add custom headers
            foreach (var header in headers)
            {
                client.DefaultRequestHeaders.Add(header.Key, header.Value);
            }

            var response = await client.PostAsJsonAsync(webhookUrl, payload);
            
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                _logger.LogError("Failed to send webhook for event {EventType}. Status: {StatusCode}, Error: {Error}",
                    eventType, response.StatusCode, error);
                return false;
            }

            _logger.LogInformation("Successfully sent webhook for event {EventType}", eventType);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending webhook for event {EventType}", eventType);
            return false;
        }
    }

    public Task<bool> IsWebhookConfiguredAsync(string eventType)
    {
        return Task.FromResult(_webhookUrls.ContainsKey(eventType));
    }

    private Dictionary<string, string> LoadWebhookUrls()
    {
        var webhookUrls = new Dictionary<string, string>();
        var webhookSection = _configuration.GetSection("Webhooks:N8n");

        if (!webhookSection.Exists())
        {
            _logger.LogWarning("No webhook configuration found in appsettings.json");
            return webhookUrls;
        }

        foreach (var webhook in webhookSection.GetChildren())
        {
            var eventType = webhook.Key;
            var url = webhook.Value;

            if (string.IsNullOrEmpty(url))
            {
                _logger.LogWarning("Webhook URL is empty for event type: {EventType}", eventType);
                continue;
            }

            webhookUrls[eventType] = url;
            _logger.LogInformation("Loaded webhook URL for event type: {EventType}", eventType);
        }

        return webhookUrls;
    }
} 