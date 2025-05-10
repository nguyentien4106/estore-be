using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;

namespace EStore.Application.Extensions;

public static class DistributedCacheExtensions
{
    public static async Task SetAsync<T>(this IDistributedCache cache, string key, T value, TimeSpan? absoluteExpiration = null, CancellationToken token = default)
    {
        var options = new DistributedCacheEntryOptions();
        if (absoluteExpiration.HasValue)
        {
            options.AbsoluteExpirationRelativeToNow = absoluteExpiration;
        }

        var jsonData = JsonSerializer.Serialize(value);
        await cache.SetStringAsync(key, jsonData, options, token);
    }

    public static async Task<T?> GetAsync<T>(this IDistributedCache cache, string key, CancellationToken token = default)
    {
        var jsonData = await cache.GetStringAsync(key, token);
        if (string.IsNullOrEmpty(jsonData))
        {
            return default;
        }

        try
        {
            return JsonSerializer.Deserialize<T>(jsonData);
        }
        catch
        {
            return default;
        }
    }

    public static T? Get<T>(this IDistributedCache cache, string key)
    {
        var jsonData = cache.GetString(key);
        if (string.IsNullOrEmpty(jsonData))
        {
            return default;
        }

        try
        {
            return JsonSerializer.Deserialize<T>(jsonData);
        }
        catch
        {
            return default;
        }
    }

    public static void Set<T>(this IDistributedCache cache, string key, T value, TimeSpan? absoluteExpiration = null)
    {
        var options = new DistributedCacheEntryOptions();
        if (absoluteExpiration.HasValue)
        {
            options.AbsoluteExpirationRelativeToNow = absoluteExpiration;
        }

        var jsonData = JsonSerializer.Serialize(value);
        cache.SetString(key, jsonData, options);
    }
} 