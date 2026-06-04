using MyTelegram.Domain.Aggregates.GroupCall;
using MyTelegram.Domain.Shared;

namespace MyTelegram.Domain.Events.GroupCall;

public class GroupCallSettingsUpdatedEvent(
    RequestInfo requestInfo,
    long callId,
    bool? joinMuted)
    : RequestAggregateEvent2<GroupCallAggregate, GroupCallId>(requestInfo)
{
    public long CallId { get; } = callId;
    public bool? JoinMuted { get; } = joinMuted;
}
