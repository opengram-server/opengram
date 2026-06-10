using System.Collections.Concurrent;

namespace MyTelegram.SessionServer.Caching;

public sealed class RequestCacheAppService : IRequestCacheAppService
{
    // Key: (authKeyId, reqMsgId) → cached result
    private readonly ConcurrentDictionary<(long AuthKeyId, long ReqMsgId), RequestCacheItem> _cache = new();
    private readonly TimeSpan _cacheExpiration = TimeSpan.FromMinutes(5);
    private long _lastCleanup;

    public void CacheResult(long authKeyId, long reqMsgId, ReadOnlyMemory<byte> resultData)
    {
        _cache[(authKeyId, reqMsgId)] = new RequestCacheItem(reqMsgId, resultData);
        TryCleanup();
    }

    public bool TryGetCachedResult(long authKeyId, long reqMsgId, out ReadOnlyMemory<byte> resultData)
    {
        if (_cache.TryGetValue((authKeyId, reqMsgId), out var item))
        {
            var now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            if (now - item.CreatedAt < (long)_cacheExpiration.TotalSeconds)
            {
                resultData = item.ResultData;
                return true;
            }

            _cache.TryRemove((authKeyId, reqMsgId), out _);
        }

        resultData = default;
        return false;
    }

    private void TryCleanup()
    {
        var now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        if (now - Interlocked.Read(ref _lastCleanup) < 60) return;
        Interlocked.Exchange(ref _lastCleanup, now);

        var expirationThreshold = now - (long)_cacheExpiration.TotalSeconds;
        foreach (var kvp in _cache)
        {
            if (kvp.Value.CreatedAt < expirationThreshold)
                _cache.TryRemove(kvp.Key, out _);
        }
    }
}
