namespace MyTelegram.Domain.Commands.Channel;

public class DeleteChannelCommand(ChannelId aggregateId,
    RequestInfo requestInfo) : RequestCommand2<ChannelAggregate, ChannelId, IExecutionResult>(aggregateId, requestInfo);