namespace MyTelegram.Domain.CommandHandlers.Messaging;

public class UnpinMessageCommandHandler : CommandHandler<MessageAggregate, MessageId, UnpinMessageCommand>
{
    public override Task ExecuteAsync(MessageAggregate aggregate, UnpinMessageCommand command, CancellationToken cancellationToken)
    {
        aggregate.UnpinMessage(command.RequestInfo);

        return Task.CompletedTask;
    }
}