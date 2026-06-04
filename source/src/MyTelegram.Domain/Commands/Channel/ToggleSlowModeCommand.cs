namespace MyTelegram.Domain.Commands.Channel;

public class ToggleSlowModeCommand(
    ChannelId aggregateId,
    RequestInfo requestInfo,
    int seconds,
    long selfUserId)
    : RequestCommand2<ChannelAggregate, ChannelId, IExecutionResult>(aggregateId, requestInfo)
{
    public int Seconds { get; } = seconds;
    public long SelfUserId { get; } = selfUserId;
}
