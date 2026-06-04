namespace MyTelegram.Domain.Events.Channel;

/// <summary>
/// Event fired when a channel is linked to a monoforum
/// </summary>
public class ChannelMonoforumLinkedEvent(
    RequestInfo requestInfo,
    long channelId,
    long linkedMonoforumId,
    bool isMonoforum,
    bool broadcastMessagesAllowed) 
    : RequestAggregateEvent2<ChannelAggregate, ChannelId>(requestInfo)
{
    public long ChannelId { get; } = channelId;
    
    /// <summary>
    /// ID of the linked monoforum (for original channel) or linked channel (for monoforum)
    /// </summary>
    public long LinkedMonoforumId { get; } = linkedMonoforumId;
    
    /// <summary>
    /// True if this channel IS a monoforum, false if it's the original channel linked TO a monoforum
    /// </summary>
    public bool IsMonoforum { get; } = isMonoforum;
    
    /// <summary>
    /// Whether broadcast messages are allowed (only for original channel)
    /// </summary>
    public bool BroadcastMessagesAllowed { get; } = broadcastMessagesAllowed;
}
