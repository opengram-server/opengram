namespace MyTelegram.Domain.Events.Messaging;

public class UpdatePinnedMessageStartedEvent(
    RequestInfo requestInfo,
    long ownerPeerId,
    int messageId,
    bool pinned,
    bool pmOneSide,
    bool silent,
    int date,
    bool isOut,
    IReadOnlyList<InboxItem> inboxItems,
    long senderPeerId,
    int senderMessageId,
    Peer toPeer,
    long randomId,
    IMessageAction messageAction
    )
    : RequestAggregateEvent2<MessageAggregate, MessageId>(requestInfo)
{
    public long OwnerPeerId { get; } = ownerPeerId;
    public int MessageId { get; } = messageId;
    public bool Pinned { get; } = pinned;
    public bool PmOneSide { get; } = pmOneSide;
    public bool Silent { get; } = silent;
    public int Date { get; } = date;
    public bool IsOut { get; } = isOut;
    public IReadOnlyList<InboxItem> InboxItems { get; } = inboxItems;
    public long SenderPeerId { get; } = senderPeerId;
    public int SenderMessageId { get; } = senderMessageId;
    public Peer ToPeer { get; } = toPeer;
    public long RandomId { get; } = randomId;
    public IMessageAction MessageAction { get; } = messageAction;
}