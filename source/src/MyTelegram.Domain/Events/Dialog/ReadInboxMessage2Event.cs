namespace MyTelegram.Domain.Events.Dialog;

public class ReadInboxMessage2Event(
    RequestInfo requestInfo,
    long readerUserId,
    long ownerPeerId,
    int maxMessageId,
    int readCount,
    int unreadCount,
    Peer toPeer,
    int date)
    : RequestAggregateEvent2<DialogAggregate, DialogId>(requestInfo)
{
    public int MaxMessageId { get; } = maxMessageId;
    public int ReadCount { get; } = readCount;
    public int UnreadCount { get; } = unreadCount;
    public Peer ToPeer { get; } = toPeer;
    public int Date { get; } = date;
    public long OwnerPeerId { get; } = ownerPeerId;
    public long ReaderUserId { get; } = readerUserId;
}