namespace MyTelegram.Domain.CommandHandlers.Temp;

public class StartDeleteParticipantHistoryCommandHandler : CommandHandler<TempAggregate, TempId, StartDeleteParticipantHistoryCommand>
{
    public override Task ExecuteAsync(TempAggregate aggregate, StartDeleteParticipantHistoryCommand command, CancellationToken cancellationToken)
    {
        aggregate.StartDeleteParticipantHistory(command.RequestInfo, command.ChannelId, command.MessageIds, command.NewTopMessageId);

        return Task.CompletedTask;
    }
}