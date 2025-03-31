using EStore.Domain.Models;
using EStore.Infrastructure.Data;
using BuildingBlocks.Auth.AuthConfiguration;
using BuildingBlocks.Behaviors;
using BuildingBlocks.Exceptions.Handler;
using Carter;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace EStore.Api;

public static class DependencyInjection
{
    public static IServiceCollection AddEStoreServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddCarter();
        services.AddExceptionHandler<CustomExceptionHandler>();
        services.AddHealthChecks();
        services.AddIdentityServices(configuration);
        services.AddJwtServices();

        return services;
    }

    private static IServiceCollection AddIdentityServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddIdentity<User, IdentityRole>(opts =>
        {
            opts.User.RequireUniqueEmail = true;
            opts.SignIn.RequireConfirmedPhoneNumber = false;
            opts.SignIn.RequireConfirmedAccount = false;
            opts.SignIn.RequireConfirmedEmail = false;
        })
        .AddEntityFrameworkStores<EStoreDbContext>()
        .AddDefaultTokenProviders();
        
        services.Configure<IdentityOptions>(options =>
        {
            options.Password.RequireDigit = false;
            options.Password.RequireLowercase = true;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequireUppercase = false;
            options.Password.RequiredLength = 6;
            options.Password.RequiredUniqueChars = 1;
        });
        
        services.Configure<DataProtectionTokenProviderOptions>(opts =>
        {
            opts.TokenLifespan = TimeSpan.FromDays(int.TryParse(configuration["JwtSettings:RefreshTokenExpirationDays"], out int days)  ? days : 10);
        });
        
        
        return services;
    }

    public static WebApplication UseEStoreApiServices(this WebApplication app)
    {
        app.UseJwtServices();
        app.MapCarter();
        app.UseExceptionHandler(opts => { });
        app.UseHealthChecks("/health", new HealthCheckOptions()
        {
            ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
        });
        
        return app;
    }
}