namespace MyTelegram.Domain.Events.Dialog;

public class ToggleDialogPinEvent(
    RequestInfo requestInfo,
    bool pinned) : RequestAggregateEvent2<DialogAggregate, DialogId>(requestInfo)
{
    public bool Pinned { get; } = pinned;
}