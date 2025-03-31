using System.Text;
using BuildingBlocks.Auth.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

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
        
        services.AddAuthorization();
        services.AddSingleton(jwtSettings);
        
        return services;
    }

    public static IApplicationBuilder UseJwtServices(this IApplicationBuilder app)
    {
        app.UseAuthentication();
        app.UseAuthorization();
        
        return app;
    }
}