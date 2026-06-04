using MyTelegram.Domain.Aggregates.GroupCall;
using MyTelegram.Domain.Shared;

namespace MyTelegram.Domain.Events.GroupCall;

public class GroupCallParticipantLeftEvent(
    RequestInfo requestInfo,
    long callId,
    long peerId,
    int source,
    int date)
    : RequestAggregateEvent2<GroupCallAggregate, GroupCallId>(requestInfo)
{
    public long CallId { get; } = callId;
    public long PeerId { get; } = peerId;
    public int Source { get; } = source;
    public int Date { get; } = date;
}
