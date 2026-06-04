namespace MyTelegram.Domain.Events.Dialog;

public class ChannelHistoryClearedEvent(
    RequestInfo requestInfo,
    long channelId,
    int historyMinId)
    : RequestAggregateEvent2<DialogAggregate, DialogId>(requestInfo)
{
    //public long OwnerPeerId { get; }
    public long ChannelId { get; } = channelId;
    public int HistoryMinId { get; } = historyMinId;
}