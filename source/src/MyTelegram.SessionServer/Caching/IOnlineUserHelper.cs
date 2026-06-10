namespace MyTelegram.SessionServer.Caching;

/// <summary>
/// Tracks online users in-memory via ConcurrentDictionary.
/// Mirrors the original binary's OnlineUserHelper.
/// </summary>
public interface IOnlineUserHelper
{
    void SetOnline(long userId, long tempAuthKeyId);
    void SetOffline(long userId, long tempAuthKeyId);
    bool IsOnline(long userId);
    IReadOnlyList<long> GetOnlineAuthKeyIds(long userId);
    int GetOnlineCount(IReadOnlyList<long> userIds);
}
