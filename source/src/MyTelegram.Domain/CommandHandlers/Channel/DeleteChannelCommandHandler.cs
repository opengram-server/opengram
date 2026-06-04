namespace MyTelegram.Domain.CommandHandlers.Channel;

public class DeleteChannelCommandHandler : CommandHandler<ChannelAggregate, ChannelId, DeleteChannelCommand>
{
    public override Task ExecuteAsync(ChannelAggregate aggregate,
        DeleteChannelCommand command,
        CancellationToken cancellationToken)
    {
        aggregate.DeleteChannel(command.RequestInfo);

        return Task.CompletedTask;
    }
}