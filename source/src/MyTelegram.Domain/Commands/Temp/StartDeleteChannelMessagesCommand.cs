namespace MyTelegram.Domain.Commands.Temp;

public class StartDeleteChannelMessagesCommand(TempId aggregateId,
    RequestInfo requestInfo,
    long channelId,
    List<int> messageIds,
    int newTopMessageId,
    int? newTopMessageIdForDiscussionGroup,
    long? discussionGroupChannelId,
    IReadOnlyCollection<int>? repliesMessageIds) : RequestCommand2<TempAggregate, TempId, IExecutionResult>(aggregateId, requestInfo)
{
    public long ChannelId { get; } = channelId;
    public List<int> MessageIds { get; } = messageIds;
    public int NewTopMessageId { get; } = newTopMessageId;
    public int? NewTopMessageIdForDiscussionGroup { get; } = newTopMessageIdForDiscussionGroup;
    public long? DiscussionGroupChannelId { get; } = discussionGroupChannelId;
    public IReadOnlyCollection<int>? RepliesMessageIds { get; } = repliesMessageIds;
}