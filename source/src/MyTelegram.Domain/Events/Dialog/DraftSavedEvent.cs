namespace MyTelegram.Domain.Events.Dialog;

public class DraftSavedEvent(
    RequestInfo requestInfo,
    long ownerPeerId,
    Peer peer,
    Draft draft)
    : RequestAggregateEvent2<DialogAggregate, DialogId>(requestInfo)
{
    public Draft Draft { get; } = draft;

    public long OwnerPeerId { get; } = ownerPeerId;
    public Peer Peer { get; } = peer;
}