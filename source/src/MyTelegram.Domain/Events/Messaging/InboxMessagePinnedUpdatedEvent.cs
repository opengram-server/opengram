namespace MyTelegram.Domain.Events.Messaging;

public class InboxMessagePinnedUpdatedEvent(
    RequestInfo requestInfo,
    long ownerPeerId,
    int messageId,
    bool pinned,
    bool pmOneSide,
    bool silent,
    int date,
    Peer toPeer,
    int pts)
    : RequestAggregateEvent2<MessageAggregate, MessageId>(requestInfo)
{
    //long channelId,

    public int Date { get; } = date;
    public Peer ToPeer { get; } = toPeer;

    public int MessageId { get; } = messageId;

    public long OwnerPeerId { get; } = ownerPeerId;
    public bool Pinned { get; } = pinned;
    public bool PmOneSide { get; } = pmOneSide;
    public int Pts { get; } = pts;
    public bool Silent { get; } = silent;
}