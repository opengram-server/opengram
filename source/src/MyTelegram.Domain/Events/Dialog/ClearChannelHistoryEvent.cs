namespace MyTelegram.Domain.Events.Dialog;

public class ClearChannelHistoryEvent(
    RequestInfo requestInfo,
    int channelHistoryMinId) : RequestAggregateEvent2<DialogAggregate, DialogId>(requestInfo)
{
    public int ChannelHistoryMinId { get; } = channelHistoryMinId;
}