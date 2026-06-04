namespace MyTelegram.Domain.Events.Dialog;

public class DialogCreatedEvent(
    long ownerId,
    Peer toPeer,
    int channelHistoryMinId,
    int topMessageId,
    DateTime creationTime,
    int? ttlPeriod = null)
    : AggregateEvent<DialogAggregate, DialogId>
{
    public int ChannelHistoryMinId { get; } = channelHistoryMinId;

    public DateTime CreationTime { get; } = creationTime;

    public long OwnerId { get; } = ownerId;
    public Peer ToPeer { get; } = toPeer;
    public int TopMessageId { get; } = topMessageId;
    public int? TtlPeriod { get; } = ttlPeriod;
}