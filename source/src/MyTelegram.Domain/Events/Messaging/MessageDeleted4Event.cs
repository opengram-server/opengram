namespace MyTelegram.Domain.Events.Messaging;

public class MessageDeleted4Event(
    RequestInfo requestInfo,
    Peer toPeer,
    long ownerPeerId,
    int messageId,
    bool isOut,
    long senderPeerId,
    int senderMessageId,
    IReadOnlyList<InboxItem>? inboxItems
)
    : AggregateEvent<MessageAggregate, MessageId>, IHasRequestInfo
{
    public IReadOnlyList<InboxItem>? InboxItems { get; } = inboxItems;
    public bool IsOut { get; } = isOut;
    public int MessageId { get; } = messageId;

    public RequestInfo RequestInfo { get; } = requestInfo;
    public Peer ToPeer { get; } = toPeer;
    public long OwnerPeerId { get; } = ownerPeerId;
    public int SenderMessageId { get; } = senderMessageId;
    public long SenderPeerId { get; } = senderPeerId;
}