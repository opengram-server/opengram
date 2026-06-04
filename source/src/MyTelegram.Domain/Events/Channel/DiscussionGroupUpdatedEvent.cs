namespace MyTelegram.Domain.Events.Channel;

public class DiscussionGroupUpdatedEvent(
    RequestInfo requestInfo,
    long broadcastChannelId,
    long? groupChannelId,
    long? oldGroupChannelId)
    : RequestAggregateEvent2<ChannelAggregate, ChannelId>(requestInfo)
{
    public long BroadcastChannelId { get; } = broadcastChannelId;
    public long? OldGroupChannelId { get; } = oldGroupChannelId;
    public long? GroupChannelId { get; } = groupChannelId;
}