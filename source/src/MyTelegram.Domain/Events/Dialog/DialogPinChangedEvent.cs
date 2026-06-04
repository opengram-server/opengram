namespace MyTelegram.Domain.Events.Dialog;

public class DialogPinChangedEvent(
    RequestInfo requestInfo,
    long ownerPeerId,
    bool pinned)
    : RequestAggregateEvent2<DialogAggregate, DialogId>(requestInfo)
{
    public long OwnerPeerId { get; } = ownerPeerId;
    public bool Pinned { get; } = pinned;
}