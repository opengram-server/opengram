namespace MyTelegram.Domain.Events.Dialog;

public class OutboxMessageHasReadEvent(
    RequestInfo requestInfo,
    int maxMessageId,
    long ownerPeerId,
    Peer toPeer)
    : RequestAggregateEvent2<DialogAggregate, DialogId>(requestInfo)
{
    public int MaxMessageId { get; } = maxMessageId;
    public long OwnerPeerId { get; } = ownerPeerId;
    public Peer ToPeer { get; } = toPeer;
}