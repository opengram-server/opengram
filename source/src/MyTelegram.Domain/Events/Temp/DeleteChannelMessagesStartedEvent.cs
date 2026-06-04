namespace MyTelegram.Domain.Events.Temp;

public class DeleteChannelMessagesStartedEvent(RequestInfo requestInfo, long channelId, List<int> messageIds, int newTopMessageId,
    int? newTopMessageIdForDiscussionGroup, long? discussionGroupChannelId, IReadOnlyCollection<int>? repliesMessageIds) : RequestAggregateEvent2<TempAggregate, TempId>(requestInfo)
{
    public long ChannelId { get; } = channelId;
    public List<int> MessageIds { get; } = messageIds;
    public int NewTopMessageId { get; } = newTopMessageId;
    public int? NewTopMessageIdForDiscussionGroup { get; } = newTopMessageIdForDiscussionGroup;
    public long? DiscussionGroupChannelId { get; } = discussionGroupChannelId;
    public IReadOnlyCollection<int>? RepliesMessageIds { get; } = repliesMessageIds;
}