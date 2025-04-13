using BuildingBlocks.Auth.AuthConfiguration;
using BuildingBlocks.Auth.Constants;
using BuildingBlocks.Auth.Models;
using Carter;
using EStore.Api.Middlewares.Files;
using EStore.Application.Constants;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Http.Features;

namespace EStore.Api.Extensions;

public static class MiddlewareExtensions
{
    public static IApplicationBuilder UseBandwidthThrottle(this IApplicationBuilder app, int bytesPerSecond)
    {
        return app.UseMiddleware<BandwidthThrottleMiddleware>(bytesPerSecond);
    }

    public static WebApplication UseEStoreApiServices(this WebApplication app)
    {
        // Configure middleware pipeline
        app.UseExceptionHandler(opts => {});
        app.UseJwtServices();

        app.UseWhen(context => {
                return context.User.HasClaim(
                    c => c.Type == ClaimNames.AccountType && 
                    c.Value == AccountType.Free.ToString()
                ) && context.Request.Path.StartsWithSegments("/files/download");
            },
            app => app.UseBandwidthThrottle(FileSizeLimits.BandwidthLimit) // 500KB/s limit
        );

        app.MapCarter();

        // Configure health checks endpoint
        app.UseHealthChecks("/health", new HealthCheckOptions
        {
            ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
        });
        
        return app;
    }
}

