namespace MyTelegram.Domain.Commands.Channel;

public class UpdateParticipantCountCommand(ChannelId aggregateId, int updatedCount)
    : Command<ChannelAggregate, ChannelId, IExecutionResult>(aggregateId)
{
    public int UpdatedCount { get; } = updatedCount;
}