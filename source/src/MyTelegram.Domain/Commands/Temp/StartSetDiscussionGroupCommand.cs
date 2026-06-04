namespace MyTelegram.Domain.Commands.Temp;

public class StartSetDiscussionGroupCommand(
    TempId aggregateId,
    RequestInfo requestInfo,
    long broadcastChannelId,
    long? discussionGroupChannelId)
    : RequestCommand2<TempAggregate, TempId, IExecutionResult>(aggregateId, requestInfo)
{
    public long BroadcastChannelId { get; } = broadcastChannelId;
    public long? DiscussionGroupChannelId { get; } = discussionGroupChannelId;
}