using System.Reflection;
using BuildingBlocks.Behaviors;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.FeatureManagement;
using EStore.Application.Services.Cloudflare;
using EStore.Application.Services.Email;
using EStore.Application.Services.Telegram;
using EStore.Application.Models.Configuration;
using EStore.Application.Services;
using EStore.Application.Services.Payment;

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
        services.AddSubscriptionMonitorService();
        
        return services;
    }

    private static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddTransient<IEmailSender<User>, EmailService>();
        services.AddTransient<ICloudflareClient, CloudflareClient>();
        services.AddSingleton<ITelegramService, TelegramService>();
        services.AddSingleton<IVnPayService, VNPayService>();

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
        
        return services;
    }

    private static IServiceCollection AddSubscriptionMonitorService(this IServiceCollection services)
    {
        services.AddHostedService<SubscriptionMonitorService>();
        
        return services;
    }
}