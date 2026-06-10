using System.Collections.Concurrent;

namespace MyTelegram.SessionServer.Caching;

/// <summary>
/// Tracks pending RPC requests so that when a DataResultResponseReceivedEvent arrives
/// (with only ReqMsgId), we can route the response to the correct client connection.
/// The original binary's DataResultResponseReceivedEvent included ConnectionId/TempAuthKeyId/SessionId
/// but the open-source version simplified the event to just ReqMsgId + Data.
/// </summary>
public sealed class PendingRequestTracker : IPendingRequestTracker
{
    private readonly ConcurrentDictionary<long, PendingRequestInfo> _pending = new();
    private long _lastCleanup;
    private readonly TimeSpan _ttl = TimeSpan.FromMinutes(5);

    public void Track(long reqMsgId, string connectionId, long authKeyId, long sessionId, int seqNumber)
    {
        _pending[reqMsgId] = new PendingRequestInfo(connectionId, authKeyId, sessionId, seqNumber,
            DateTimeOffset.UtcNow.ToUnixTimeSeconds());
        TryCleanup();
    }

    public bool TryGet(long reqMsgId, out PendingRequestInfo info)
    {
        if (_pending.TryRemove(reqMsgId, out info!))
        {
            var now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            if (now - info.CreatedAt < (long)_ttl.TotalSeconds)
                return true;
        }

        info = default!;
        return false;
    }

    private void TryCleanup()
    {
        var now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        if (now - Interlocked.Read(ref _lastCleanup) < 60) return;
        Interlocked.Exchange(ref _lastCleanup, now);

        var threshold = now - (long)_ttl.TotalSeconds;
        foreach (var kvp in _pending)
        {
            if (kvp.Value.CreatedAt < threshold)
                _pending.TryRemove(kvp.Key, out _);
        }
    }
}

public record PendingRequestInfo(
    string ConnectionId,
    long AuthKeyId,
    long SessionId,
    int SeqNumber,
    long CreatedAt);

public interface IPendingRequestTracker
{
    void Track(long reqMsgId, string connectionId, long authKeyId, long sessionId, int seqNumber);
    bool TryGet(long reqMsgId, out PendingRequestInfo info);
}
