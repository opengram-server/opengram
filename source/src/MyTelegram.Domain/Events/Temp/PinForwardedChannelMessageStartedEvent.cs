namespace MyTelegram.Domain.Events.Temp;

public class PinForwardedChannelMessageStartedEvent
    (RequestInfo requestInfo, long channelId, int messageId) : RequestAggregateEvent2<TempAggregate, TempId>(
    requestInfo)
{
    public long ChannelId { get; } = channelId;
    public int MessageId { get; } = messageId;
}