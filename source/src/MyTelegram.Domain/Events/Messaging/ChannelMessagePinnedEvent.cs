namespace MyTelegram.Domain.Events.Messaging;

public class ChannelMessagePinnedEvent(RequestInfo requestInfo, long channelId, int messageId)
    : AggregateEvent<MessageAggregate, MessageId>,
        IHasRequestInfo
{
    public long ChannelId { get; } = channelId;
    public int MessageId { get; } = messageId;

    public RequestInfo RequestInfo { get; } = requestInfo;
}