using System.Collections.Concurrent;
using Microsoft.Extensions.Options;
using MyTelegram.SessionServer.Options;

namespace MyTelegram.SessionServer.Caching;

/// <summary>
/// Reconstructed from original binary. Holds auth keys in a
/// <see cref="ConcurrentDictionary{TKey,TValue}"/> keyed by authKeyId (temp or perm).
/// On cache miss, falls back to MongoDB via EventFlow IQueryProcessor (not implemented
/// in this pass — will be wired when the EventFlow read-models are registered).
/// </summary>
public sealed class AuthKeyHelper : IAuthKeyHelper
{
    private readonly ConcurrentDictionary<long, AuthKeyItem> _cache = new();
    private readonly ILogger<AuthKeyHelper> _logger;
    private readonly IOptionsMonitor<MyTelegramSessionServerOptions> _options;

    public AuthKeyHelper(
        ILogger<AuthKeyHelper> logger,
        IOptionsMonitor<MyTelegramSessionServerOptions> options)
    {
        _logger = logger;
        _options = options;
    }

    public bool TryGetAuthKeyItem(long authKeyId, out AuthKeyItem? item)
        => _cache.TryGetValue(authKeyId, out item);

    public AuthKeyItem? GetAuthKeyItem(long authKeyId)
        => _cache.GetValueOrDefault(authKeyId);

    public void SetAuthKeyItem(long authKeyId, AuthKeyItem item)
        => _cache[authKeyId] = item;

    public void RemoveAuthKeyItem(long authKeyId)
        => _cache.TryRemove(authKeyId, out _);

    public void UpdateUserId(long authKeyId, long userId)
    {
        if (_cache.TryGetValue(authKeyId, out var item))
            item.UserId = userId;
    }

    public void UpdateLayer(long authKeyId, int layer)
    {
        if (_cache.TryGetValue(authKeyId, out var item))
            item.Layer = layer;
    }

    public void UpdateServerSalt(long authKeyId, long serverSalt)
    {
        if (_cache.TryGetValue(authKeyId, out var item))
            item.ServerSalt = serverSalt;
    }

    public void UpdateAccessHashKeyId(long authKeyId, long accessHashKeyId)
    {
        if (_cache.TryGetValue(authKeyId, out var item))
            item.AccessHashKeyId = accessHashKeyId;
    }

    public void UpdateDeviceType(long authKeyId, DeviceType deviceType)
    {
        if (_cache.TryGetValue(authKeyId, out var item))
            item.DeviceType = deviceType;
    }

    public long GetPermAuthKeyId(long tempAuthKeyId)
    {
        if (_cache.TryGetValue(tempAuthKeyId, out var item))
            return item.PermAuthKeyId;
        return 0;
    }

    public IReadOnlyList<long> GetAllTempAuthKeyIds()
    {
        return _cache
            .Where(kvp => kvp.Value.TempAuthKeyId != 0)
            .Select(kvp => kvp.Key)
            .ToList();
    }

    public void BindTempAuthKey(long tempAuthKeyId, long permAuthKeyId)
    {
        if (_cache.TryGetValue(tempAuthKeyId, out var item))
        {
            item.PermAuthKeyId = permAuthKeyId;

            // Copy userId from perm key if available
            if (_cache.TryGetValue(permAuthKeyId, out var permItem) && permItem.UserId > 0)
            {
                item.UserId = permItem.UserId;
            }
        }
        else
        {
            // Create minimal temp item
            var newItem = new AuthKeyItem(
                permAuthKeyId: permAuthKeyId,
                tempAuthKeyId: tempAuthKeyId,
                userId: 0, data: ReadOnlyMemory<byte>.Empty,
                serverSalt: 0, isActive: true, layer: 0,
                accessHashKeyId: 0, deviceType: null, expiresAt: 0, isMediaTempAuthKey: false);

            if (_cache.TryGetValue(permAuthKeyId, out var permItem2))
            {
                newItem.UserId = permItem2.UserId;
                newItem.AccessHashKeyId = permItem2.AccessHashKeyId;
            }

            _cache[tempAuthKeyId] = newItem;
        }

        _logger.LogDebug("Simple bind temp auth key {TempAuthKeyId} → perm {PermAuthKeyId}",
            tempAuthKeyId, permAuthKeyId);
    }

    public void BindTempAuthKey(
        long tempAuthKeyId,
        long permAuthKeyId,
        ReadOnlyMemory<byte> tempKeyData,
        long tempServerSalt,
        int expiresAt,
        DeviceType? deviceType,
        bool isMedia)
    {
        // Get or create the perm key entry and copy userId
        long userId = 0;
        long accessHashKeyId = 0;
        if (_cache.TryGetValue(permAuthKeyId, out var permItem))
        {
            userId = permItem.UserId;
            accessHashKeyId = permItem.AccessHashKeyId != 0
                ? permItem.AccessHashKeyId
                : Random.Shared.NextInt64();
        }

        var tempItem = new AuthKeyItem(
            permAuthKeyId: permAuthKeyId,
            tempAuthKeyId: tempAuthKeyId,
            userId: userId,
            data: tempKeyData,
            serverSalt: tempServerSalt,
            isActive: true,
            layer: 0,
            accessHashKeyId: accessHashKeyId,
            deviceType: deviceType,
            expiresAt: expiresAt,
            isMediaTempAuthKey: isMedia);

        _cache[tempAuthKeyId] = tempItem;

        _logger.LogInformation(
            "Bound temp auth key {TempAuthKeyId} to perm {PermAuthKeyId}, expires at {ExpiresAt}",
            tempAuthKeyId, permAuthKeyId, expiresAt);
    }

    public async Task StartRemoveExpiredTempAuthKeyTimerAsync(CancellationToken ct)
    {
        var interval = TimeSpan.FromMinutes(5);
        _logger.LogInformation("Started expired temp auth key cleanup timer (interval: {Interval})", interval);

        while (!ct.IsCancellationRequested)
        {
            try
            {
                await Task.Delay(interval, ct).ConfigureAwait(false);

                var now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
                var expiredKeys = new List<long>();

                foreach (var kvp in _cache)
                {
                    // Only check temp keys (ExpiresAt > 0 means it's a temp key)
                    if (kvp.Value.ExpiresAt > 0 && kvp.Value.ExpiresAt <= now)
                    {
                        expiredKeys.Add(kvp.Key);
                    }
                }

                foreach (var key in expiredKeys)
                {
                    _cache.TryRemove(key, out _);
                }

                if (expiredKeys.Count > 0)
                {
                    _logger.LogInformation("Removed {Count} expired temp auth keys", expiredKeys.Count);
                }
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing expired temp auth keys");
            }
        }
    }
}
