namespace MyTelegram.Domain.Events.Messaging;

public class SelfMessageDeletedEvent(
    RequestInfo requestInfo,
    long ownerPeerId,
    int messageId,
    bool isOut,
    long senderPeerId,
    int senderMessageId,
    IReadOnlyList<InboxItem>? inboxItems)
    : RequestAggregateEvent2<MessageAggregate, MessageId>(requestInfo)
{
    public IReadOnlyList<InboxItem>? InboxItems { get; } = inboxItems;
    public bool IsOut { get; } = isOut;
    public int MessageId { get; } = messageId;

    public long OwnerPeerId { get; } = ownerPeerId;
    public int SenderMessageId { get; } = senderMessageId;
    public long SenderPeerId { get; } = senderPeerId;
}