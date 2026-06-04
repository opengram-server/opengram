namespace MyTelegram.Domain.Events.Dialog;

public class SetOutboxTopMessageSuccessEvent(
    int messageId,
    long ownerPeerId,
    Peer toPeer,
    bool clearDraft)
    : AggregateEvent<DialogAggregate, DialogId>
{
    public bool ClearDraft { get; } = clearDraft;

    public int MessageId { get; } = messageId;
    public long OwnerPeerId { get; } = ownerPeerId;
    public Peer ToPeer { get; } = toPeer;
}