namespace MyTelegram.Domain.Events.Dialog;

/// <summary>
/// Event fired when dialog history TTL (Time-To-Live) is updated
/// </summary>
public class DialogHistoryTTLUpdatedEvent(
    RequestInfo requestInfo,
    long ownerPeerId,
    Peer peer,
    int? ttlPeriod) 
    : RequestAggregateEvent2<DialogAggregate, DialogId>(requestInfo)
{
    public long OwnerPeerId { get; } = ownerPeerId;
    public Peer Peer { get; } = peer;
    
    /// <summary>
    /// TTL period in seconds. null = disabled
    /// </summary>
    public int? TtlPeriod { get; } = ttlPeriod;
}
