using Microsoft.Extensions.Caching.Distributed;
using MyTelegram.Core;

namespace MyTelegram.Caching.Redis;

public class CacheManager<TCacheItem>(
    IDistributedCache distributedCache,
    ICacheSerializer cacheSerializer)
    : ICacheManager<TCacheItem>
    where TCacheItem : class
{
    public async Task<TCacheItem?> GetAsync(string key)
    {
        var cachedBytes = await distributedCache.GetAsync(key);
        if (cachedBytes == null)
        {
            return default;
        }

        return cacheSerializer.Deserialize<TCacheItem?>(cachedBytes);
        //return JsonSerializer.Deserialize<TCacheItem>(Encoding.UTF8.GetString(cachedBytes), _jsonSerializerOptions);
    }

    public async Task<IDictionary<string, TCacheItem>> GetManyAsync(IReadOnlyList<string> keys)
    {
        var cachedDict = new Dictionary<string, TCacheItem>();
        foreach (var key in keys)
        {
            var cacheItem = await GetAsync(key);
            if (cacheItem == null)
            {
                continue;
            }
            cachedDict.TryAdd(key, cacheItem);
        }

        return cachedDict;
    }

    public Task RemoveAsync(string key)
    {
        return distributedCache.RemoveAsync(key);
    }

    public Task SetAsync(string key,
        TCacheItem value,
        int ttlInSeconds = -1)
    {
        DistributedCacheEntryOptions? cacheOptions;
        if (ttlInSeconds > 0)
        {
            cacheOptions = new()
            {
                SlidingExpiration = TimeSpan.FromSeconds(ttlInSeconds)
            };
        }
        else
        {
            cacheOptions = new DistributedCacheEntryOptions
            {
                SlidingExpiration = null
            };
        }

        //var bytes = JsonSerializer.SerializeToUtf8Bytes(value, _jsonSerializerOptions);
        var bytes = cacheSerializer.Serialize(value);
        return distributedCache.SetAsync(key, bytes, cacheOptions);
    }
}