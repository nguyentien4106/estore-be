using BuildingBlocks.Auth.Constants;
using BuildingBlocks.Auth.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace BuildingBlocks.Auth.AuthConfiguration;

public static class EStoreExtensions
{
    public static IServiceCollection AddJwtServices(this IServiceCollection services)
    {
        var jwtSettings = JwtSettingsReader.GetJwtSettings();
        
        services
        .AddAuthentication(opts =>
        {
            opts.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            opts.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(opts =>
        {
            opts.Audience = jwtSettings.Audience;
            opts.Authority = jwtSettings.EStoreority;
            opts.RequireHttpsMetadata = false;
            opts.TokenValidationParameters = Constants.Constants.GetTokenValidationParameters(jwtSettings);
        });

        services.AddCors(opts =>
        {
            opts.AddPolicy("web", policy =>
            {
                policy.AllowAnyOrigin()
                .AllowAnyHeader()
                .AllowAnyMethod()
                .WithExposedHeaders("Content-Disposition");
            });
        });

        services.AddAuthorizationBuilder()
            .AddPolicy("RequireProOrAbove", policy =>
            {
                policy.RequireAssertion(context =>
                {
                    return context.User.HasClaim(ClaimNames.AccountType, AccountType.Pro.ToString()) ||
                           context.User.HasClaim(ClaimNames.AccountType, AccountType.Plus.ToString());
                });
            })
            .AddPolicy("RequirePlus", policy =>
            {
                policy.RequireAssertion(context =>
                {
                    return context.User.HasClaim(ClaimNames.AccountType, AccountType.Plus.ToString());
                });
            });

        services.AddSingleton(jwtSettings);
        
        return services;
    }

    public static IApplicationBuilder UseJwtServices(this IApplicationBuilder app)
    {
        app.UseCors("web");
        app.UseAuthentication();
        app.UseAuthorization();
        
        return app;
    }
}