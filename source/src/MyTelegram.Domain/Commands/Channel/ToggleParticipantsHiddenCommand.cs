namespace MyTelegram.Domain.Commands.Channel;

public class ToggleParticipantsHiddenCommand(ChannelId aggregateId, RequestInfo requestInfo, bool enabled) : RequestCommand2<ChannelAggregate, ChannelId, IExecutionResult>(aggregateId, requestInfo)
{
    public bool Enabled { get; } = enabled;
}