using EStore.Application.Extensions;
using EStore.Application.Queries.Files.GetR2FilePreview;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace EStore.Application.Services.R2PresignUrl;

public class CachedR2PresignUrlService(
    IR2PresignUrlService inner,
    IDistributedCache cache,
    ILogger<CachedR2PresignUrlService> logger) : IR2PresignUrlService
{
    private static readonly TimeSpan CacheDuration = TimeSpan.FromDays(7);

    public async Task<AppResponse<R2PresignUrlResponse>> GetPresignedUrlAsync(Guid id)
    {
        var cacheKey = $"r2_presign_{id}";

        try
        {
            // Try to get from cache
            var cachedResponse = await cache.GetAsync<AppResponse<R2PresignUrlResponse>>(cacheKey);
            if (cachedResponse != null && cachedResponse.Data != null && cachedResponse.Data.ExpiresAt > DateTime.UtcNow)
            {
                logger.LogInformation("Cache hit for presigned URL with ID: {Id}", id);
                return cachedResponse;
            }

            // Get from inner service
            var response = await inner.GetPresignedUrlAsync(id);

            // Cache the response if successful
            if (response.Succeed && response.Data != null)
            {
                await cache.SetAsync(cacheKey, response, CacheDuration);
                logger.LogInformation("Cached presigned URL for ID: {Id}", id);
            }
            else
            {
                logger.LogWarning("Failed to generate presigned URL for ID: {Id}. Error: {Error}",
                    id, response.Message);
            }

            return response;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error while handling presigned URL request for ID: {Id}", id);
            return AppResponse<R2PresignUrlResponse>.Error($"Cache operation failed: {ex.Message}");
        }
    }
} 