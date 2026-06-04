namespace MyTelegram.Domain.CommandHandlers.Channel;

public class ToggleParticipantsHiddenCommandHandler : CommandHandler<ChannelAggregate, ChannelId, ToggleParticipantsHiddenCommand>
{
    public override Task ExecuteAsync(ChannelAggregate aggregate,
        ToggleParticipantsHiddenCommand command,
        CancellationToken cancellationToken)
    {
        aggregate.ToggleParticipantsHidden(command.RequestInfo, command.Enabled);

        return Task.CompletedTask;
    }
}