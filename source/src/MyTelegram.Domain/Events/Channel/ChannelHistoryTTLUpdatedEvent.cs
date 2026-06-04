namespace MyTelegram.Domain.Events.Channel;

/// <summary>
/// Event fired when channel history TTL is updated
/// </summary>
public class ChannelHistoryTTLUpdatedEvent(
    RequestInfo requestInfo,
    long channelId,
    int? ttlPeriod) 
    : RequestAggregateEvent2<ChannelAggregate, ChannelId>(requestInfo)
{
    public long ChannelId { get; } = channelId;
    
    /// <summary>
    /// TTL period in seconds. null = disabled
    /// </summary>
    public int? TtlPeriod { get; } = ttlPeriod;
}
