using MyTelegram.Domain.Aggregates.GroupCall;
using MyTelegram.Domain.Shared;

namespace MyTelegram.Domain.Events.GroupCall;

public class GroupCallParticipantUpdatedEvent(
    RequestInfo requestInfo,
    long callId,
    long peerId,
    bool? muted,
    bool mutedByAdmin,
    int? volume,
    bool? raiseHand,
    bool? videoStopped,
    int date)
    : RequestAggregateEvent2<GroupCallAggregate, GroupCallId>(requestInfo)
{
    public long CallId { get; } = callId;
    public long PeerId { get; } = peerId;
    public bool? Muted { get; } = muted;
    public bool MutedByAdmin { get; } = mutedByAdmin;
    public int? Volume { get; } = volume;
    public bool? RaiseHand { get; } = raiseHand;
    public bool? VideoStopped { get; } = videoStopped;
    public int Date { get; } = date;
}
