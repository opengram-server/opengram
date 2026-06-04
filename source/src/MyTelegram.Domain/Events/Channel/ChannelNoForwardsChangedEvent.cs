namespace MyTelegram.Domain.Events.Channel;

public class ChannelNoForwardsChangedEvent(RequestInfo requestInfo, long channelId, bool enabled)
    : RequestAggregateEvent2<ChannelAggregate, ChannelId>(requestInfo)
{
    public long ChannelId { get; } = channelId;
    public bool Enabled { get; } = enabled;
}