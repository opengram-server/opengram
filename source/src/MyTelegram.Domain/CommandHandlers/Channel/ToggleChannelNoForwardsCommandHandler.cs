namespace MyTelegram.Domain.CommandHandlers.Channel;

public class
    ToggleChannelNoForwardsCommandHandler : CommandHandler<ChannelAggregate, ChannelId, ToggleChannelNoForwardsCommand>
{
    public override Task ExecuteAsync(ChannelAggregate aggregate, ToggleChannelNoForwardsCommand command,
        CancellationToken cancellationToken)
    {
        aggregate.ToggleNoForwards(command.RequestInfo,command.Enabled);
        return Task.CompletedTask;
    }
}