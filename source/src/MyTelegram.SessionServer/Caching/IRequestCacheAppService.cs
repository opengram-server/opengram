namespace MyTelegram.SessionServer.Caching;

/// <summary>
/// RPC request deduplication cache.
/// </summary>
public interface IRequestCacheAppService
{
    void CacheResult(long authKeyId, long reqMsgId, ReadOnlyMemory<byte> resultData);
    bool TryGetCachedResult(long authKeyId, long reqMsgId, out ReadOnlyMemory<byte> resultData);
}
