namespace MyTelegram.Domain.Events.Temp;

public class DraftDeletedEvent(long ownerPeerId, Peer toPeer) : AggregateEvent<TempAggregate, TempId>
{
    public long OwnerPeerId { get; } = ownerPeerId;
    public Peer ToPeer { get; } = toPeer;
}