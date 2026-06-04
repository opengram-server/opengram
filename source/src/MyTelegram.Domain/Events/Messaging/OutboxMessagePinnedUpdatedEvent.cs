namespace MyTelegram.Domain.Events.Messaging;

public class OutboxMessagePinnedUpdatedEvent(
    RequestInfo requestInfo,
    long ownerPeerId,
    int messageId,
    bool pinned,
    bool pmOneSide,
    bool silent,
    int date,
    IReadOnlyList<InboxItem> inboxItems,
    long senderPeerId,
    int senderMessageId,
    Peer toPeer,
    int pts,
    bool post
    )
    : RequestAggregateEvent2<MessageAggregate, MessageId>(requestInfo)
{
    //long channelId,
    //ChannelId = channelId;

    public int Date { get; } = date;
    public IReadOnlyList<InboxItem> InboxItems { get; } = inboxItems;

    public int MessageId { get; } = messageId;

    public long OwnerPeerId { get; } = ownerPeerId;

    //public long ChannelId { get; }
    public bool Pinned { get; } = pinned;
    public bool PmOneSide { get; } = pmOneSide;
    public int Pts { get; } = pts;
    public bool Post { get; } = post;
    public int SenderMessageId { get; } = senderMessageId;
    public Peer ToPeer { get; } = toPeer;
    public long SenderPeerId { get; } = senderPeerId;
    public bool Silent { get; } = silent;
}