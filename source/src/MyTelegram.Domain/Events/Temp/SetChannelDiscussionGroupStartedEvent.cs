namespace MyTelegram.Domain.Events.Temp;

public class SetChannelDiscussionGroupStartedEvent(
    RequestInfo requestInfo,
    long broadcastChannelId,
    long? discussionGroupChannelId)
    : RequestAggregateEvent2<TempAggregate, TempId>(requestInfo)
{
    public long BroadcastChannelId { get; } = broadcastChannelId;
    public long? DiscussionGroupChannelId { get; } = discussionGroupChannelId;
}