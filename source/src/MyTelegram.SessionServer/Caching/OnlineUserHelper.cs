using System.Collections.Concurrent;

namespace MyTelegram.SessionServer.Caching;

public sealed class OnlineUserHelper : IOnlineUserHelper
{
    // userId → set of tempAuthKeyIds currently online
    private readonly ConcurrentDictionary<long, ConcurrentDictionary<long, byte>> _onlineUsers = new();

    public void SetOnline(long userId, long tempAuthKeyId)
    {
        var keys = _onlineUsers.GetOrAdd(userId, _ => new ConcurrentDictionary<long, byte>());
        keys[tempAuthKeyId] = 1;
    }

    public void SetOffline(long userId, long tempAuthKeyId)
    {
        if (_onlineUsers.TryGetValue(userId, out var keys))
        {
            keys.TryRemove(tempAuthKeyId, out _);
            if (keys.IsEmpty)
                _onlineUsers.TryRemove(userId, out _);
        }
    }

    public bool IsOnline(long userId)
        => _onlineUsers.TryGetValue(userId, out var keys) && !keys.IsEmpty;

    public IReadOnlyList<long> GetOnlineAuthKeyIds(long userId)
    {
        if (_onlineUsers.TryGetValue(userId, out var keys))
            return keys.Keys.ToList();
        return Array.Empty<long>();
    }

    public int GetOnlineCount(IReadOnlyList<long> userIds)
    {
        var count = 0;
        foreach (var uid in userIds)
        {
            if (IsOnline(uid))
                count++;
        }
        return count;
    }
}
