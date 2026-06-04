namespace MyTelegram.Domain.CommandHandlers.Temp;

public class
    StartUpdatePinnedMessagesCommandHandler : CommandHandler<TempAggregate, TempId, StartUpdatePinnedMessagesCommand>
{
    public override Task ExecuteAsync(TempAggregate aggregate, StartUpdatePinnedMessagesCommand command,
        CancellationToken cancellationToken)
    {
        aggregate.StartUpdatePinnedMessages(command.RequestInfo, command.MessageItems, command.ToPeer, command.Pinned, command.PmOneSide);

        return Task.CompletedTask;
    }
}