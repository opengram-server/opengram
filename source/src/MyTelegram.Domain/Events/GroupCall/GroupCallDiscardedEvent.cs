using MyTelegram.Domain.Aggregates.GroupCall;
using MyTelegram.Domain.Shared;

namespace MyTelegram.Domain.Events.GroupCall;

public class GroupCallDiscardedEvent(
    RequestInfo requestInfo,
    long callId,
    long peerId,
    PeerType peerType,
    int date)
    : RequestAggregateEvent2<GroupCallAggregate, GroupCallId>(requestInfo)
{
    public long CallId { get; } = callId;
    public long PeerId { get; } = peerId;
    public PeerType PeerType { get; } = peerType;
    public int Date { get; } = date;
}
