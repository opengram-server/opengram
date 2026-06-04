namespace MyTelegram.Domain.Events.Channel;

public class ChannelEmojiStickersUpdatedEvent(
    RequestInfo requestInfo,
    long channelId,
    long? stickerSetId)
    : RequestAggregateEvent2<ChannelAggregate, ChannelId>(requestInfo)
{
    public long ChannelId { get; } = channelId;
    public long? StickerSetId { get; } = stickerSetId;
}
