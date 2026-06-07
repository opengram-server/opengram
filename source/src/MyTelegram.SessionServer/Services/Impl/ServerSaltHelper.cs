using Microsoft.Extensions.Options;
using MyTelegram.Core;
using MyTelegram.SessionServer.Options;
using StackExchange.Redis;
using System.Text.Json;

namespace MyTelegram.SessionServer.Services.Impl;

/// <summary>
/// Reconstructed from the original binary's ServerSaltHelper.
/// Generates future salts with 2-hour validity windows and caches them in Redis.
/// </summary>
public sealed class ServerSaltHelper : IServerSaltHelper
{
    private readonly int _maxSaltCount;
    private readonly IDatabase _redisDb;
    private readonly ILogger<ServerSaltHelper> _logger;

    public ServerSaltHelper(
        IConnectionMultiplexer redis,
        IOptionsMonitor<MyTelegramSessionServerOptions> options,
        ILogger<ServerSaltHelper> logger)
    {
        _redisDb = redis.GetDatabase();
        _maxSaltCount = options.CurrentValue.MaxFutureSaltCount;
        _logger = logger;
    }

    public async Task<List<FutureSaltCacheItem>> GetOrCreateCachedFutureSaltsAsync(
        long tempAuthKeyId, int count)
    {
        var cacheKey = FutureSaltCacheItem.GetCacheKey(tempAuthKeyId);

        var cached = await _redisDb.StringGetAsync(cacheKey).ConfigureAwait(false);
        if (cached.HasValue)
        {
            try
            {
                var result = JsonSerializer.Deserialize<List<FutureSaltCacheItem>>(cached.ToString());
                if (result != null && result.Count > 0)
                    return result;
            }
            catch (JsonException ex)
            {
                _logger.LogWarning(ex, "Failed to deserialize cached future salts for {AuthKeyId}", tempAuthKeyId);
            }
        }

        var requested = Math.Min(count, _maxSaltCount);
        var nowUnix = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        const int windowSeconds = 7200; // 2 hours
        var ttlSeconds = requested * windowSeconds;

        var freshSalts = new List<FutureSaltCacheItem>(requested);
        for (var i = 0; i < requested; i++)
        {
            var salt = Random.Shared.NextInt64();
            freshSalts.Add(new FutureSaltCacheItem(
                Salt: salt,
                ValidSince: (int)(nowUnix + i * windowSeconds),
                ValidUntil: (int)(nowUnix + (i + 1) * windowSeconds)));
        }

        await _redisDb.StringSetAsync(
            cacheKey,
            JsonSerializer.Serialize(freshSalts),
            TimeSpan.FromSeconds(ttlSeconds)
        ).ConfigureAwait(false);

        return freshSalts;
    }
}
