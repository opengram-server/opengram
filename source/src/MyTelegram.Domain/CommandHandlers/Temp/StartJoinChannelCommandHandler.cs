namespace MyTelegram.Domain.CommandHandlers.Temp;

public class StartJoinChannelCommandHandler : CommandHandler<TempAggregate, TempId, StartJoinChannelCommand>
{
    public override Task ExecuteAsync(TempAggregate aggregate, StartJoinChannelCommand command, CancellationToken cancellationToken)
    {
        aggregate.StartJoinChannel(command.RequestInfo, command.ChannelId, command.Broadcast, command.TopMessageId, command.ChannelHistoryMinId);

        return Task.CompletedTask;
    }
}