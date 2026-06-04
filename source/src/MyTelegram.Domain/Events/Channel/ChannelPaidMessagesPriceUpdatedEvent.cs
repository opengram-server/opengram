namespace MyTelegram.Domain.Events.Channel;

/// <summary>
/// Event fired when channel paid messages price is updated
/// </summary>
public class ChannelPaidMessagesPriceUpdatedEvent(
    RequestInfo requestInfo,
    long channelId,
    long? sendPaidMessagesStars,
    bool broadcastMessagesAllowed,
    long? linkedMonoforumId = null) 
    : RequestAggregateEvent2<ChannelAggregate, ChannelId>(requestInfo)
{
    public long ChannelId { get; } = channelId;
    
    /// <summary>
    /// Price in stars to send paid messages. null = disabled
    /// </summary>
    public long? SendPaidMessagesStars { get; } = sendPaidMessagesStars;
    
    /// <summary>
    /// Whether broadcast messages are allowed
    /// </summary>
    public bool BroadcastMessagesAllowed { get; } = broadcastMessagesAllowed;
    
    /// <summary>
    /// ID of the linked monoforum (special supergroup for direct messages)
    /// </summary>
    public long? LinkedMonoforumId { get; } = linkedMonoforumId;
}
