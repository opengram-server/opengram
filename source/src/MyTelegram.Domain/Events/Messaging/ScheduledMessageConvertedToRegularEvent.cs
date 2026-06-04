namespace MyTelegram.Domain.Events.Messaging;

public class ScheduledMessageConvertedToRegularEvent(
    RequestInfo requestInfo,
    long ownerPeerId,
    int messageId,
    Peer toPeer,
    MessageItem messageItem)
    : RequestAggregateEvent2<MessageAggregate, MessageId>(requestInfo)
{
    public long OwnerPeerId { get; } = ownerPeerId;
    public int MessageId { get; } = messageId;
    public Peer ToPeer { get; } = toPeer;
    public MessageItem MessageItem { get; } = messageItem;
}
