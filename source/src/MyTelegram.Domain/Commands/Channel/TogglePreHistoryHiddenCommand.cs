namespace MyTelegram.Domain.Commands.Channel;

public class TogglePreHistoryHiddenCommand(
    ChannelId aggregateId,
    RequestInfo requestInfo,
    bool hidden,
    long selfUserId)
    : RequestCommand2<ChannelAggregate, ChannelId, IExecutionResult>(aggregateId, requestInfo)
{
    public bool Hidden { get; } = hidden;
    public long SelfUserId { get; } = selfUserId;
}
