using System.Reflection;
using BuildingBlocks.Behaviors;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.FeatureManagement;
using EStore.Application.Services.Cloudflare;
using EStore.Application.Services.Email;
using EStore.Application.Services.Telegram;
using EStore.Application.Services.Payment;
using EStore.Application.Services.R2PresignUrl;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using EStore.Application.Services.RabbitMQ;
using EStore.Application.Services.BackgroundServices;
namespace EStore.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
            cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
            cfg.AddOpenBehavior(typeof(LoggingBehavior<,>));
        });
        
        services.AddConfigurationObjects(configuration);
        services.AddFeatureManagement();
        services.AddServices();
        services.AddWorkerServices();
        services.AddR2PresignedUrlServices(configuration);
        
        return services;
    }

    private static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddTransient<IEmailSender<User>, EmailService>();
        services.AddTransient<ICloudflareClient, CloudflareClient>();
        services.AddSingleton<ITelegramService, TelegramService>();
        services.AddTransient<IVnPayService, VNPayService>();
        services.AddTransient<IRabbitMQService, RabbitMQService>();
        //services.AddTransient<IWebhookService, N8nWebhookService>();

        return services;
    }

    public static IServiceCollection AddConfigurationObjects(this IServiceCollection services, IConfiguration configuration)
    {
        // Register configuration instances for DI
        services.AddSingleton(configuration.GetSection("SendGridSettings")
            .Get<SendGridConfiguration>() ?? throw new InvalidOperationException("SendGridSettings not found."));
        
        services.AddSingleton(configuration.GetSection("CloudflareConfiguration")
            .Get<CloudflareConfiguration>() ?? throw new InvalidOperationException("CloudflareConfiguration not found."));
        
        services.AddSingleton(configuration.GetSection("TelegramConfiguration")
            .Get<TelegramConfiguration>() ?? throw new InvalidOperationException("TelegramConfiguration not found."));

        services.AddSingleton(configuration.GetSection("VNPayConfiguration")
            .Get<VNPayConfiguration>() ?? throw new InvalidOperationException("VNPayConfiguration not found."));
        
        services.AddSingleton(configuration.GetSection("RabbitMQConfiguration")
            .Get<RabbitMQConfiguration>() ?? throw new InvalidOperationException("RabbitMQConfiguration not found."));
        
        services.AddSingleton(configuration.GetSection("Webhooks")
            .Get<WebhooksConfiguration>() ?? throw new InvalidOperationException("WebhooksConfiguration not found."));

        services.AddSingleton(configuration.GetSection("AppSettings")
            .Get<AppSettings>() ?? throw new InvalidOperationException("AppSettings not found."));

        return services;
    }

    private static IServiceCollection AddWorkerServices(this IServiceCollection services)
    {
        services.AddHostedService<PushFileWorkerService>();
        services.AddHostedService<MergeFileWorkerService>();
        services.AddHostedService<PushFileWorkerService>();
        return services;
    }


    private static IServiceCollection AddR2PresignedUrlServices(this IServiceCollection services, IConfiguration configuration)
    {
        var a = configuration.GetConnectionString("Redis") ?? throw new InvalidOperationException("Redis connection string not found.");
        // Configure Redis
        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = configuration.GetConnectionString("Redis") ??  throw new InvalidOperationException("Redis connection string not found.");
        });

        // Register R2 presigned URL services
        services.AddScoped<R2PresignUrlService>();
        services.AddScoped<IR2PresignUrlService>(provider =>
        {
            var baseService = provider.GetRequiredService<R2PresignUrlService>();
            var cache = provider.GetRequiredService<IDistributedCache>();
            var logger = provider.GetRequiredService<ILogger<CachedR2PresignUrlService>>();
            
            return new CachedR2PresignUrlService(baseService, cache, logger);
        });

        return services;
    }
}