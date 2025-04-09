using EStore.Domain.Models;
using EStore.Infrastructure.Data;
using BuildingBlocks.Auth.AuthConfiguration;
using BuildingBlocks.Exceptions.Handler;
using Carter;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Identity;
using FluentValidation;
using EStore.Api.Middlewares;
using EStore.Application.Constants;
using Microsoft.AspNetCore.Authorization;
using BuildingBlocks.Auth.Models;

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
        services.AddValidators();

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

    private static IServiceCollection AddValidators(this IServiceCollection services)
    {
        var assembly = typeof(Program).Assembly;
        services.AddValidatorsFromAssembly(assembly);

        return services;
    }


    public static IServiceCollection AddAuthorizationHandlers(this IServiceCollection services)
    {
        services.AddAuthorizationBuilder()
            .AddPolicy("FreeTierFileSizeLimit", policy =>
                policy.Requirements.Add(new FileSizeLimitRequirement(FileSizeLimits.FreeTierLimit)))
            .AddPolicy("ProTierFileSizeLimit", policy =>
                policy.Requirements.Add(new FileSizeLimitRequirement(FileSizeLimits.ProTierLimit)))
            .AddPolicy("PlusTierFileSizeLimit", policy =>
                policy.Requirements.Add(new FileSizeLimitRequirement(FileSizeLimits.PlusTierLimit)));

        services.AddAuthorizationBuilder()
            .AddPolicy("RequirePro", policy =>
            {
                policy.Requirements.Add(new AccountRequirement(AccountType.Pro.ToString()));
            })
            .AddPolicy("RequirePlus", policy =>
            {
                policy.Requirements.Add(new AccountRequirement(AccountType.Plus.ToString()));
            });

        services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        services.AddSingleton<IAuthorizationHandler, FileSizeLimitAuthorizationHandler>();
        services.AddSingleton<IAuthorizationHandler, AccountAuthorizationHandler>();
        services.AddSingleton<IAuthorizationMiddlewareResultHandler, CustomAuthorizationMiddlewareResultHandler>();
        return services;
    }
    
    public static WebApplication UseEStoreApiServices(this WebApplication app)
    {

        app.UseExceptionHandler(opts => {});
        app.UseJwtServices();
        app.MapCarter();
        app.UseHealthChecks("/health", new HealthCheckOptions()
        {
            ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
        });
        
        return app;
    }
}