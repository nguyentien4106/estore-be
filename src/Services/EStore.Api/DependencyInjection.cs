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
using EStore.Api.Middlewares.Files;
using EStore.Api.Middlewares.Auth;
using EStore.Api.Extensions;
using Microsoft.AspNetCore.Http.Features;

namespace EStore.Api;

public static class DependencyInjection
{
    public static IServiceCollection AddEStoreServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddCarter();
        services.AddExceptionHandler<CustomExceptionHandler>();
        services.AddHealthChecks();
        services.AddIdentityServices(configuration);
        services.AddJwtServices();
        services.AddValidators();
        services.AddAuthorizationHandlers();
        services.ConfigureFormOptions();

        return services;
    }

    private static IServiceCollection ConfigureFormOptions(this IServiceCollection services)
    {
        services.Configure<FormOptions>(options =>
        {
            // Set form value length to maximum
            options.ValueLengthLimit = int.MaxValue;
            
            // Set multipart body length to Plus tier limit (5 GB)
            options.MultipartBodyLengthLimit = FileSizeLimits.PlusTierLimit;
        });

        return services;
    }

    private static IServiceCollection AddIdentityServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Configure Identity user settings
        services.AddIdentity<User, IdentityRole>(opts =>
        {
            opts.User.RequireUniqueEmail = true;
            opts.SignIn.RequireConfirmedPhoneNumber = false;
            opts.SignIn.RequireConfirmedAccount = false;
            opts.SignIn.RequireConfirmedEmail = false;
        })
        .AddEntityFrameworkStores<EStoreDbContext>()
        .AddDefaultTokenProviders();
        
        // Configure password requirements
        services.Configure<IdentityOptions>(options =>
        {
            options.Password.RequireDigit = false;
            options.Password.RequireLowercase = true;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequireUppercase = false;
            options.Password.RequiredLength = 6;
            options.Password.RequiredUniqueChars = 1;
        });
        
        // Configure token provider options
        services.Configure<DataProtectionTokenProviderOptions>(opts =>
        {
            opts.TokenLifespan = TimeSpan.FromDays(
                int.TryParse(configuration["JwtSettings:RefreshTokenExpirationDays"], out int days) 
                    ? days 
                    : 10
            );
        });
        
        return services;
    }

    private static IServiceCollection AddValidators(this IServiceCollection services)
    {
        services.AddValidatorsFromAssembly(typeof(Program).Assembly);
        return services;
    }

    private static IServiceCollection AddAuthorizationHandlers(this IServiceCollection services)
    {
        // Configure authorization policies
        services.AddAuthorizationBuilder()
            // File size limit policies
            .AddPolicy("FreeTierFileSizeLimit", policy =>
                policy.Requirements.Add(new FileSizeLimitRequirement(FileSizeLimits.FreeTierLimit)))
            .AddPolicy("ProTierFileSizeLimit", policy =>
                policy.Requirements.Add(new FileSizeLimitRequirement(FileSizeLimits.ProTierLimit)))
            .AddPolicy("PlusTierFileSizeLimit", policy =>
                policy.Requirements.Add(new FileSizeLimitRequirement(FileSizeLimits.PlusTierLimit)))
            // Account type policies  
            .AddPolicy("RequirePro", policy => 
                policy.Requirements.Add(new AccountRequirement(AccountType.Pro.ToString())))
            .AddPolicy("RequirePlus", policy =>
                policy.Requirements.Add(new AccountRequirement(AccountType.Plus.ToString())));

        // Register authorization services
        services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        services.AddSingleton<IAuthorizationHandler, FileSizeLimitAuthorizationHandler>();
        services.AddSingleton<IAuthorizationHandler, AccountAuthorizationHandler>();
        services.AddSingleton<IAuthorizationMiddlewareResultHandler, CustomAuthorizationMiddlewareResultHandler>();

        return services;
    }
    
}