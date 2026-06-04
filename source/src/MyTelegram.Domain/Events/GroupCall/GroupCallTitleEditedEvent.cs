using MyTelegram.Domain.Aggregates.GroupCall;

namespace MyTelegram.Domain.Events.GroupCall;

public class GroupCallTitleEditedEvent(
    RequestInfo requestInfo,
    long callId,
    string title,
    int date)
    : RequestAggregateEvent2<GroupCallAggregate, GroupCallId>(requestInfo)
{
    public long CallId { get; } = callId;
    public string Title { get; } = title;
    public int Date { get; } = date;
}
