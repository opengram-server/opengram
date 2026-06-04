namespace MyTelegram.Domain.Events.Dialog;

public class ReadOutboxMaxIdUpdatedEvent(RequestInfo requestInfo, long ownerUserId, long toPeerId, int readOutboxMaxId)
    : RequestAggregateEvent2<DialogAggregate, DialogId>(requestInfo)
{
    public long OwnerUserId { get; } = ownerUserId;
    public long ToPeerId { get; } = toPeerId;
    public int ReadOutboxMaxId { get; } = readOutboxMaxId;
}