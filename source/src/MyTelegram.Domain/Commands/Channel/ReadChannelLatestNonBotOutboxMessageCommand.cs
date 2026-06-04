namespace MyTelegram.Domain.Commands.Channel;

public class ReadChannelLatestNonBotOutboxMessageCommand(
    ChannelId aggregateId,
    RequestInfo requestInfo,
    string sourceCommandId)
    : RequestCommand2<ChannelAggregate, ChannelId, IExecutionResult>(aggregateId, requestInfo) //,
//IHasCorrelationId
{
    public string SourceCommandId { get; } = sourceCommandId;
}
