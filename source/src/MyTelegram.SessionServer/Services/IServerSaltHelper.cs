using MyTelegram.Core;

namespace MyTelegram.SessionServer.Services;

/// <summary>
/// Generates and caches future salts (2-hour windows) per temp auth key.
/// Uses a distributed cache (Redis) with fallback to generation.
/// </summary>
public interface IServerSaltHelper
{
    Task<List<FutureSaltCacheItem>> GetOrCreateCachedFutureSaltsAsync(long tempAuthKeyId, int count);
}
