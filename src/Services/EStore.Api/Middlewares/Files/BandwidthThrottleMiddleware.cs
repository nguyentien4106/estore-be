using Estore.Application.Dtos.Files;

namespace EStore.Api.Middlewares.Files;

public class BandwidthThrottleMiddleware
{
    private readonly RequestDelegate _next;
    private readonly int _bytesPerSecond;

    public BandwidthThrottleMiddleware(RequestDelegate next, int bytesPerSecond)
    {
        _next = next;
        _bytesPerSecond = bytesPerSecond;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var response = context.Response;
        var originalBodyStream = response.Body;

        try
        {
            using var throttledStream = new ThrottledStream(originalBodyStream, _bytesPerSecond);
            response.Body = throttledStream;
            await _next(context);
        }
        finally
        {
            response.Body = originalBodyStream;
        }
    }
}