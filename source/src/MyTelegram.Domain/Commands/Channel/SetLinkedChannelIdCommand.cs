namespace MyTelegram.Domain.Commands.Channel;

public class SetLinkedChannelIdCommand(ChannelId aggregateId, RequestInfo requestInfo, long? linkedChannelId)
    : Command<ChannelAggregate, ChannelId, IExecutionResult>(aggregateId), IHasRequestInfo
{
    public long? LinkedChannelId { get; } = linkedChannelId;

    public RequestInfo RequestInfo { get; } = requestInfo;
}