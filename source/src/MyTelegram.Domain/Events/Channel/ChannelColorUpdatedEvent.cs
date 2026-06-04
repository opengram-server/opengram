namespace MyTelegram.Domain.Events.Channel;

public class ChannelColorUpdatedEvent(
    RequestInfo requestInfo,
    long channelId,
    PeerColor color,
    long? backgroundEmojiId,
    bool forProfile)
    : RequestAggregateEvent2<ChannelAggregate, ChannelId>(requestInfo)
{
    public long ChannelId { get; } = channelId;
    public PeerColor Color { get; } = color;
    public long? BackgroundEmojiId { get; } = backgroundEmojiId;
    public bool ForProfile { get; } = forProfile;
}