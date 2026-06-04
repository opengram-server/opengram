using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;

namespace MyTelegram.Services.Services;

public class AckCacheService(IScheduleAppService scheduleAppService) : IAckCacheService, ISingletonDependency
{
    private readonly ConcurrentDictionary<long, AckCacheItem> _msgIdToPtsDict = new();
    private readonly ConcurrentDictionary<long, long> _msgIdToReqMsgIdDict = new();
    private readonly ConcurrentDictionary<long, AckCacheItem> _rpcReqMsgIdToPtsDict = new();
    private readonly int _timeoutSeconds = 60 * 10;

    public Task AddMsgIdToCacheAsync(long msgId,
        int ptsOrQts,
        long globalSeqNo,
        Peer toPeer, bool isQts = false)
    {
        _msgIdToPtsDict.TryAdd(msgId, new AckCacheItem(ptsOrQts, globalSeqNo, toPeer, isQts));
        scheduleAppService.Execute(() =>
            {
                _msgIdToPtsDict.TryRemove(msgId, out _);
            },
            TimeSpan.FromSeconds(_timeoutSeconds));
        return Task.CompletedTask;
    }

    public void AddRpcMsgIdToCache(long msgId,
            long reqMsgId)
    {
        _msgIdToReqMsgIdDict.TryAdd(msgId, reqMsgId);
        scheduleAppService.Execute(() => _msgIdToReqMsgIdDict.TryRemove(msgId, out _), TimeSpan.FromSeconds(50));
    }

    public Task AddRpcPtsToCacheAsync(long reqMsgId,
        int pts,
        long globalSeqNo,
        Peer toPeer,
        bool isFromGetDifference)
    {
        _rpcReqMsgIdToPtsDict.TryAdd(reqMsgId, new AckCacheItem(pts, globalSeqNo, toPeer, false, isFromGetDifference));
        scheduleAppService.Execute(() =>
            {
                _rpcReqMsgIdToPtsDict.TryRemove(reqMsgId, out _);
            },
            TimeSpan.FromSeconds(_timeoutSeconds));
        return Task.CompletedTask;
    }

    public bool TryGetPts(long msgId,
        [NotNullWhen(true)] out AckCacheItem? ackCacheItem)
    {
        return _msgIdToPtsDict.TryRemove(msgId, out ackCacheItem);
    }

    public bool TryGetRpcPtsCache(long msgId,
            [NotNullWhen(true)] out AckCacheItem? ackRpcCacheItem)
    {
        if (_msgIdToReqMsgIdDict.TryRemove(msgId, out var reqMsgId))
        {
            return _rpcReqMsgIdToPtsDict.TryRemove(reqMsgId, out ackRpcCacheItem);
        }

        ackRpcCacheItem = null;
        return false;
    }
}