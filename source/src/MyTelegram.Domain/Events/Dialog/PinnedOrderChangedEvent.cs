namespace MyTelegram.Domain.Events.Dialog;

public class PinnedOrderChangedEvent(int order) : AggregateEvent<DialogAggregate, DialogId>
{
    public int Order { get; } = order;
}