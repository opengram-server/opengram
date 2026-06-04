namespace MyTelegram.Domain.Commands.Channel;

public class ToggleChannelNoForwardsCommand(ChannelId aggregateId, RequestInfo requestInfo, bool enabled)
    : RequestCommand2<ChannelAggregate, ChannelId, IExecutionResult>(aggregateId, requestInfo) //, IHasCorrelationId
{
    public bool Enabled { get; } = enabled;
}