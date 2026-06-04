namespace MyTelegram.Domain.Events.Dialog;

public class DialogFilterUpdatedEvent(RequestInfo requestInfo, long ownerUserId,
    DialogFilter filter)
    : RequestAggregateEvent2<DialogFilterAggregate, DialogFilterId>(requestInfo)
{
    public long OwnerUserId { get; } = ownerUserId;
    public DialogFilter Filter { get; } = filter;
}