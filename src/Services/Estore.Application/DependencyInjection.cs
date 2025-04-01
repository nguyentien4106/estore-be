using System.Reflection;
using EStore.Application.Services;
using EStore.Domain.Models;
using BuildingBlocks.Behaviors;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.FeatureManagement;
using Estore.Application.Services;

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
        
        var sendGridSettings = configuration.GetSection("SendGridSettings").Get<SendGridSettings>() ?? throw new InvalidOperationException("SendGridSettings not found.");
        services.AddSingleton(sendGridSettings);

        var cloudflareConfiguration = configuration.GetSection("CloudflareConfiguration").Get<CloudflareConfiguration>() ?? throw new InvalidOperationException("Cloudflare Configuration not found.");
        services.AddSingleton(cloudflareConfiguration);

        var telegramConfiguration = configuration.GetSection("TelegramConfiguration").Get<TelegramConfiguration>() ?? throw new InvalidOperationException("Cloudflare Configuration not found.");
        services.AddSingleton(telegramConfiguration);

        services.AddFeatureManagement();
        services.AddServices();
        
        return services;
    }

    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddSingleton<IEmailSender<User>, EmailService>();
        services.AddScoped<ICloudflareClient, CloudflareClient>();
        services.AddSingleton<ITelegramService, TelegramService>();

        return services;
    }
}