namespace MyTelegram.Domain.Events.Channel;

public class ChannelTopMessageIdUpdatedEvent(
    long channelId,
    int topMessageId)
    : AggregateEvent<ChannelAggregate, ChannelId>
{
    public long ChannelId { get; } = channelId;
    public int TopMessageId { get; } = topMessageId;
}