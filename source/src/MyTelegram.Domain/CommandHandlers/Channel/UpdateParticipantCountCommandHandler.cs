namespace MyTelegram.Domain.CommandHandlers.Channel;

public class
    UpdateParticipantCountCommandHandler : CommandHandler<ChannelAggregate, ChannelId, UpdateParticipantCountCommand>
{
    public override Task ExecuteAsync(ChannelAggregate aggregate, UpdateParticipantCountCommand command,
        CancellationToken cancellationToken)
    {
        aggregate.UpdateParticipantCount(command.UpdatedCount);

        return Task.CompletedTask;
    }
}