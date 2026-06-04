namespace MyTelegram.Messenger.Services.Caching;

public class CacheAppService
{
    private readonly HashSet<Int128> _caches = [];

    public Task<bool> IsExistsAsync(long selfUserId, long targetUserId)
    {
        return Task.FromResult(_caches.Contains(GetKey(selfUserId, targetUserId)));
    }

    public void Add(long selfUserId, long targetUserId)
    {
        _caches.Add(GetKey(selfUserId, targetUserId));
    }

    public void Remove(long selfUserId, long targetUserId)
    {
        _caches.Add(GetKey(selfUserId, targetUserId));
    }

    private Int128 GetKey(long selfUserId, long targetUserId)
    {
        return new Int128((ulong)selfUserId, (ulong)targetUserId);
    }
}