namespace MyTelegram.Domain.CommandHandlers.Temp;

public class StartDeleteMessagesCommandHandler : CommandHandler<TempAggregate, TempId, StartDeleteMessagesCommand>
{
    public override Task ExecuteAsync(TempAggregate aggregate, StartDeleteMessagesCommand command, CancellationToken cancellationToken)
    {
        aggregate.StartDeleteMessages(command.RequestInfo, command.MessageItems, command.Revoke, command.DeleteGroupMessagesForEveryone, command.NewTopMessageId, command.NewTopMessageIdForOtherParticipant);

        return Task.CompletedTask;
    }
}