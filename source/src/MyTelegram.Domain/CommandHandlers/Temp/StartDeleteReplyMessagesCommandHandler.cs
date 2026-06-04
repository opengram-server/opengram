namespace MyTelegram.Domain.CommandHandlers.Temp;

public class
    StartDeleteReplyMessagesCommandHandler : CommandHandler<TempAggregate, TempId, StartDeleteReplyMessagesCommand>
{
    public override Task ExecuteAsync(TempAggregate aggregate, StartDeleteReplyMessagesCommand command,
        CancellationToken cancellationToken)
    {
        aggregate.StartDeleteReplyMessages(command.RequestInfo, command.ChannelId, command.MessageIds, command.NewTopMessageId);

        return Task.CompletedTask;
    }
}