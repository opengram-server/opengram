namespace MyTelegram.Domain.Events.Messaging;

public class InboxMessageDeletedEvent(
    RequestInfo requestInfo,
    long ownerPeerId,
    int messageId,
    int senderMessageId)
    : AggregateEvent<MessageAggregate, MessageId>, IHasRequestInfo
{
    public int MessageId { get; } = messageId;
    public int SenderMessageId { get; } = senderMessageId;
    public long OwnerPeerId { get; } = ownerPeerId;
    public RequestInfo RequestInfo { get; } = requestInfo;
}