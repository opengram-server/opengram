namespace MyTelegram.Domain.Events.Dialog;

public class ReadInboxMaxIdUpdatedEvent(
    RequestInfo requestInfo,
    long ownerUserId,
    long toPeerId,
    int readInboxMaxId,
    long senderUserId,
    int senderMessageId,
    int unreadCount
    )
    : RequestAggregateEvent2<DialogAggregate, DialogId>(requestInfo)
{
    public long OwnerUserId { get; } = ownerUserId;
    public long ToPeerId { get; } = toPeerId;
    public int ReadInboxMaxId { get; } = readInboxMaxId;
    public long SenderUserId { get; } = senderUserId;
    public int SenderMessageId { get; } = senderMessageId;
    public int UnreadCount { get; } = unreadCount;
}