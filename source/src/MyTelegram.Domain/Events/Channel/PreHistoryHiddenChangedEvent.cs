namespace MyTelegram.Domain.Events.Channel;

public class PreHistoryHiddenChangedEvent(
    RequestInfo requestInfo,
    long channelId,
    bool hidden)
    : RequestAggregateEvent2<ChannelAggregate, ChannelId>(requestInfo)
{
    public long ChannelId { get; } = channelId;
    public bool Hidden { get; } = hidden;
}
