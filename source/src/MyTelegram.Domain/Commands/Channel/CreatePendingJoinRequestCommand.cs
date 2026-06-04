namespace MyTelegram.Domain.Commands.Channel;

public class CreatePendingJoinRequestCommand(ChannelId aggregateId, RequestInfo requestInfo,
    long requestUserId
    ) : RequestCommand2<ChannelAggregate, ChannelId, IExecutionResult>(aggregateId, requestInfo)
{
    public long RequestUserId { get; } = requestUserId;
}