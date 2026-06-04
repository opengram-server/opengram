using MyTelegram.Domain.Aggregates.GroupCall;

namespace MyTelegram.Domain.Events.GroupCall;

/// <summary>
/// Event emitted when a scheduled group call is started
/// </summary>
public class GroupCallStartedEvent(
    RequestInfo requestInfo,
    long callId,
    int date)
    : RequestAggregateEvent2<GroupCallAggregate, GroupCallId>(requestInfo)
{
    public long CallId { get; } = callId;
    public int Date { get; } = date;
}
