namespace MyTelegram.Domain.Events.Temp;

public class UnpinAllMessagesStartedEvent(
    RequestInfo requestInfo,
    IReadOnlyCollection<SimpleMessageItem> messageItems,
    Peer toPeer) : RequestAggregateEvent2<TempAggregate, TempId>(requestInfo)
{
    public IReadOnlyCollection<SimpleMessageItem> MessageItems { get; } = messageItems;
    public Peer ToPeer { get; } = toPeer;
}