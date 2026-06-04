namespace MyTelegram.Domain.Events.Channel;

public class LinkedChannelChangedEvent(
    RequestInfo requestInfo,
    long channelId,
    long? linkedChannelId,
    long? oldLinkedChannelId)
    : AggregateEvent<ChannelAggregate, ChannelId>, IHasRequestInfo
{
    public long ChannelId { get; } = channelId;
    public long? LinkedChannelId { get; } = linkedChannelId;
    public long? OldLinkedChannelId { get; } = oldLinkedChannelId;

    public RequestInfo RequestInfo { get; } = requestInfo;
}