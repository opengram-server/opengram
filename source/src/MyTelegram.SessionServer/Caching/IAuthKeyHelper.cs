namespace MyTelegram.SessionServer.Caching;

/// <summary>
/// In-memory cache of authentication keys. Falls back to <c>IQueryProcessor</c>
/// (MongoDB) on cache miss. This mirrors the original binary's AuthKeyHelper,
/// which uses a <see cref="ConcurrentDictionary{TKey,TValue}"/> — <b>not</b> Redis.
/// </summary>
public interface IAuthKeyHelper
{
    bool TryGetAuthKeyItem(long authKeyId, out AuthKeyItem? item);
    AuthKeyItem? GetAuthKeyItem(long authKeyId);
    void SetAuthKeyItem(long authKeyId, AuthKeyItem item);
    void RemoveAuthKeyItem(long authKeyId);

    void UpdateUserId(long authKeyId, long userId);
    void UpdateLayer(long authKeyId, int layer);
    void UpdateServerSalt(long authKeyId, long serverSalt);
    void UpdateAccessHashKeyId(long authKeyId, long accessHashKeyId);
    void UpdateDeviceType(long authKeyId, DeviceType deviceType);

    long GetPermAuthKeyId(long tempAuthKeyId);
    void BindTempAuthKey(long tempAuthKeyId, long permAuthKeyId);
    void BindTempAuthKey(long tempAuthKeyId, long permAuthKeyId, ReadOnlyMemory<byte> tempKeyData,
        long tempServerSalt, int expiresAt, DeviceType? deviceType, bool isMedia);

    /// <summary>Returns all temp auth key IDs currently in the cache.</summary>
    IReadOnlyList<long> GetAllTempAuthKeyIds();

    /// <summary>
    /// Starts a periodic timer that removes expired temporary auth keys.
    /// Called once during startup.
    /// </summary>
    Task StartRemoveExpiredTempAuthKeyTimerAsync(CancellationToken ct);
}
