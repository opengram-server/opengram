namespace MyTelegram.Domain.Commands.Dialog;

public class ReadInboxMessageCommand2(DialogId aggregateId,
        RequestInfo requestInfo,
        long readerUserId,
        long ownerPeerId,
        int maxMessageId,
        int unreadCount,
        Peer toPeer, int date)
    : Command<DialogAggregate, DialogId, IExecutionResult>(aggregateId), IHasRequestInfo
{
    public int MaxMessageId { get; } = maxMessageId;
    public int UnreadCount { get; } = unreadCount;
    public long OwnerPeerId { get; } = ownerPeerId;
    public long ReaderUserId { get; } = readerUserId;

    public Peer ToPeer { get; } = toPeer;
    public int Date { get; } = date;

    public RequestInfo RequestInfo { get; } = requestInfo;
}
