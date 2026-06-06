namespace MyTelegram.SessionServer.Caching;

/// <summary>
/// Cached RPC request result for deduplication.
/// If an identical request arrives within the cache window,
/// the cached result is returned without re-processing.
/// </summary>
public sealed class RequestCacheItem
{
    public long ReqMsgId { get; set; }
    public ReadOnlyMemory<byte> ResultData { get; set; }
    public long CreatedAt { get; set; }

    public RequestCacheItem(long reqMsgId, ReadOnlyMemory<byte> resultData)
    {
        ReqMsgId = reqMsgId;
        ResultData = resultData;
        CreatedAt = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
    }
}
