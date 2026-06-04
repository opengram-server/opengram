namespace MyTelegram.Domain.CommandHandlers.Messaging;

public class ConvertScheduledMessageToRegularCommandHandler : 
    CommandHandler<MessageAggregate, MessageId, ConvertScheduledMessageToRegularCommand>
{
    public override Task ExecuteAsync(
        MessageAggregate aggregate,
        ConvertScheduledMessageToRegularCommand command,
        CancellationToken cancellationToken)
    {
        aggregate.ConvertScheduledMessageToRegular(command.RequestInfo);
        return Task.CompletedTask;
    }
}
