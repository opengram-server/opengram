namespace MyTelegram.Domain.CommandHandlers.Channel;

public class
    ToggleJoinRequestCommandHandler : CommandHandler<ChannelAggregate, ChannelId, ToggleJoinRequestCommand>
{
    public override Task ExecuteAsync(ChannelAggregate aggregate, ToggleJoinRequestCommand command,
        CancellationToken cancellationToken)
    {
        aggregate.ToggleJoinRequest(command.RequestInfo, command.Enabled);
        return Task.CompletedTask;
    }
}