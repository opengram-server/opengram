namespace MyTelegram.Domain.Commands.Channel;

public class SetDiscussionGroupCommand(
    ChannelId aggregateId,
    RequestInfo requestInfo,
    long broadcastChannelId,
    long? groupChannelId)
    : RequestCommand2<ChannelAggregate, ChannelId, IExecutionResult>(aggregateId, requestInfo)
{
    public long BroadcastChannelId { get; } = broadcastChannelId;
    public long? GroupChannelId { get; } = groupChannelId;
}
