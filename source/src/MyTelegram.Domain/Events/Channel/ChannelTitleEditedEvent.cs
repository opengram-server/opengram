namespace MyTelegram.Domain.Events.Channel;

public class ChannelTitleEditedEvent(
    RequestInfo requestInfo,
    long channelId,
    bool broadcast,
    string title,
    long? linkedChannelId,
    IMessageAction messageAction,
    long randomId)
    : RequestAggregateEvent2<ChannelAggregate, ChannelId>(requestInfo)
{
    public long ChannelId { get; } = channelId;
    public bool Broadcast { get; } = broadcast;
    public long RandomId { get; } = randomId;
    public string Title { get; } = title;
    public long? LinkedChannelId { get; } = linkedChannelId;
    public IMessageAction MessageAction { get; } = messageAction;
}
