namespace MyTelegram.Domain.Events.Channel;

public class SlowModeChangedEvent(
    RequestInfo requestInfo,
    long channelId,
    int seconds)
    : RequestAggregateEvent2<ChannelAggregate, ChannelId>(requestInfo)
{
    public long ChannelId { get; } = channelId;
    public int Seconds { get; } = seconds;
}
