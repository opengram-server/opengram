namespace MyTelegram.Domain.Events.Dialog;

public class DialogMsgIdPinnedEvent(int pinnedMsgId) : AggregateEvent<DialogAggregate, DialogId>
{
    public int PinnedMsgId { get; } = pinnedMsgId;
}