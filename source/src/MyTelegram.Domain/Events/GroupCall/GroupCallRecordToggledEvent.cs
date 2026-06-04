using MyTelegram.Domain.Aggregates.GroupCall;

namespace MyTelegram.Domain.Events.GroupCall;

public class GroupCallRecordToggledEvent(
    RequestInfo requestInfo,
    long callId,
    bool start,
    bool video,
    string? title,
    bool? videoPortrait,
    int date)
    : RequestAggregateEvent2<GroupCallAggregate, GroupCallId>(requestInfo)
{
    public long CallId { get; } = callId;
    public bool Start { get; } = start;
    public bool Video { get; } = video;
    public string? Title { get; } = title;
    public bool? VideoPortrait { get; } = videoPortrait;
    public int Date { get; } = date;
}
