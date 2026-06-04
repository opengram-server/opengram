namespace MyTelegram.Domain.Events.Channel;

public class NewMsgIdPinnedEvent(
    int pinnedMsgId,
    bool pinned) : AggregateEvent<ChannelAggregate, ChannelId>
{
    public bool Pinned { get; } = pinned;

    public int PinnedMsgId { get; } = pinnedMsgId;
}
