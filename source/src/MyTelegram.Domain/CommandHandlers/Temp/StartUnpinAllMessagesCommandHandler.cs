namespace MyTelegram.Domain.CommandHandlers.Temp;

public class StartUnpinAllMessagesCommandHandler : CommandHandler<TempAggregate, TempId, StartUnpinAllMessagesCommand>
{
    public override Task ExecuteAsync(TempAggregate aggregate, StartUnpinAllMessagesCommand command, CancellationToken cancellationToken)
    {
        aggregate.StartUnpinAllMessages(command.RequestInfo, command.MessageItems, command.ToPeer);

        return Task.CompletedTask;
    }
}