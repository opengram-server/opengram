namespace MyTelegram.Domain.Events.Messaging;

public class MessageReplyUpdatedEvent(long ownerChannelId, long channelId, int messageId, int pts)
    : AggregateEvent<MessageAggregate, MessageId>
{
    public long OwnerChannelId { get; } = ownerChannelId;
    public long ChannelId { get; } = channelId;
    public int MessageId { get; } = messageId;
    public int Pts { get; } = pts;
}