using EStore.Application.Services.Webhooks;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net.Http.Headers;

namespace EStore.Api.Extensions // Corrected namespace
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddCustomWebhookServices(this IServiceCollection services) // Renamed to avoid potential conflicts
        {
            // This registers IHttpClientFactory and configures a named HttpClient
            services.AddHttpClient("n8n-webhook", client =>
            {
                client.Timeout = TimeSpan.FromSeconds(30);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            });

            services.AddScoped<IWebhookService, N8nWebhookService>();

            return services;
        }

        // Add other service registration extension methods here if needed
    }
} 