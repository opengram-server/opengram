namespace MyTelegram.Domain.Events.Dialog;

public class ParticipantHistoryClearedEvent(
    RequestInfo requestInfo,
    long ownerPeerId,
    int historyMinId)
    : RequestAggregateEvent2<DialogAggregate, DialogId>(requestInfo)
{
    public int HistoryMinId { get; } = historyMinId;

    public long OwnerPeerId { get; } = ownerPeerId;
}