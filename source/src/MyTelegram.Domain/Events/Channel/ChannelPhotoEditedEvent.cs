namespace MyTelegram.Domain.Events.Channel;

public class ChannelPhotoEditedEvent(
    RequestInfo requestInfo,
    long channelId,
    bool broadcast,
    long? photoId,
    IMessageAction messageAction,
    long? linkedChannelId,
    long randomId)
    : RequestAggregateEvent2<ChannelAggregate, ChannelId>(requestInfo)
{
    public long ChannelId { get; } = channelId;
    public bool Broadcast { get; } = broadcast;
    public long? PhotoId { get; } = photoId;
    public IMessageAction MessageAction { get; } = messageAction;
    public long? LinkedChannelId { get; } = linkedChannelId;
    public long RandomId { get; } = randomId;
}
