namespace MyTelegram.Domain.Commands.Channel;

public class SetChannelHistoryTTLCommand(
    ChannelId aggregateId,
    RequestInfo requestInfo,
    int? ttlPeriod)
    : RequestCommand2<ChannelAggregate, ChannelId, IExecutionResult>(aggregateId, requestInfo)
{
    public int? TtlPeriod { get; } = ttlPeriod;
}
