using System.Text.Json;
using LearningPlatform.Shared.Settings;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;

namespace LearningPlatform.Infrastructure.Cache;

public class CacheService(IDistributedCache distributedCache, IOptions<CacheSettings> cacheSettings) : ICacheService
{
    private readonly CacheSettings _settings = cacheSettings.Value;

    public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default)
    {
        var cached = await distributedCache.GetStringAsync(key, cancellationToken);
        return cached is null ? default : JsonSerializer.Deserialize<T>(cached);
    }

    public async Task SetAsync<T>(
        string key,
        T value,
        TimeSpan? expiration = null,
        CancellationToken cancellationToken = default)
    {
        var options = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = expiration
                ?? TimeSpan.FromMinutes(_settings.DefaultExpirationMinutes)
        };

        await distributedCache.SetStringAsync(key, JsonSerializer.Serialize(value), options, cancellationToken);
    }

    public Task RemoveAsync(string key, CancellationToken cancellationToken = default) =>
        distributedCache.RemoveAsync(key, cancellationToken);

    public async Task<T> GetOrCreateAsync<T>(
        string key,
        Func<CancellationToken, Task<T>> factory,
        TimeSpan? expiration = null,
        CancellationToken cancellationToken = default)
    {
        var cached = await GetAsync<T>(key, cancellationToken);
        if (cached is not null)
            return cached;

        var value = await factory(cancellationToken);
        await SetAsync(key, value, expiration, cancellationToken);
        return value;
    }
}
