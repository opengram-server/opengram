namespace MyTelegram.Domain.Events.Dialog;

public class TopMessageIdUpdatedEvent(long ownerUserId, Peer toPeer, int newTopMessageId)
    : AggregateEvent<DialogAggregate, DialogId>
{
    public long OwnerUserId { get; } = ownerUserId;
    public Peer ToPeer { get; } = toPeer;
    public int NewTopMessageId { get; } = newTopMessageId;
}