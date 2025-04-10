using BuildingBlocks.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.AspNetCore.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace EStore.Api.Middlewares;

public class CustomAuthorizationMiddlewareResultHandler : IAuthorizationMiddlewareResultHandler
{
    private readonly AuthorizationMiddlewareResultHandler _defaultHandler = new();
    private readonly ILogger<CustomAuthorizationMiddlewareResultHandler> _logger;
    private readonly JsonSerializerOptions _jsonSerializerOptions;

    public CustomAuthorizationMiddlewareResultHandler(ILogger<CustomAuthorizationMiddlewareResultHandler> logger)
    {
        _logger = logger;
        // Configure JsonSerializerOptions to use camelCase
        _jsonSerializerOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            // Optionally ignore null values
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };
    }

    public async Task HandleAsync(RequestDelegate next, HttpContext context, AuthorizationPolicy policy, PolicyAuthorizationResult authorizeResult)
    {
        if (!authorizeResult.Succeeded)
        {
            // Check if there is failure metadata in the AuthorizationHandlerContext
            var failureMetadata = authorizeResult.AuthorizationFailure?.FailureReasons
                .Select(r => r.Message)
                .FirstOrDefault();

            var metadata = context.Items["AuthorizationFailure"] as AppResponse<string>;

            if (metadata != null)
            {
                _logger.LogWarning("Authorization failed: {Reason} - {Message}", metadata.Data, metadata.Message);
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                context.Response.ContentType = "application/json";

                await context.Response.WriteAsync(JsonSerializer.Serialize(metadata, _jsonSerializerOptions));
                return;
            }

            // Fallback if no metadata is found
            _logger.LogWarning("Authorization failed with no specific reason provided.");
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            context.Response.ContentType = "application/json";

            var fallbackResponse = AppResponse<string>.Error("Access denied.", "AUTHORIZATION_FAILED");

            await context.Response.WriteAsync(JsonSerializer.Serialize(fallbackResponse, _jsonSerializerOptions));
            return;
        }

        // If authorization succeeds, delegate to the default handler
        await _defaultHandler.HandleAsync(next, context, policy, authorizeResult);
    }
}