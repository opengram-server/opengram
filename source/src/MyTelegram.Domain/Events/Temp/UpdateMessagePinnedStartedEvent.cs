namespace MyTelegram.Domain.Events.Temp;

public class UpdateMessagePinnedStartedEvent(
    RequestInfo requestInfo,
    IReadOnlyCollection<SimpleMessageItem> messageItems,
    Peer toPeer, bool pinned, bool pmOneSide) : RequestAggregateEvent2<TempAggregate, TempId>(requestInfo)
{
    public IReadOnlyCollection<SimpleMessageItem> MessageItems { get; } = messageItems;
    public Peer ToPeer { get; } = toPeer;
    public bool Pinned { get; } = pinned;
    public bool PmOneSide { get; } = pmOneSide;
}