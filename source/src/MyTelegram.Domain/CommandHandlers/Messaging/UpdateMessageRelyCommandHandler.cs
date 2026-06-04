namespace MyTelegram.Domain.CommandHandlers.Messaging;

public class UpdateMessageRelyCommandHandler : CommandHandler<MessageAggregate, MessageId, UpdateMessageRelyCommand>
{
    public override Task ExecuteAsync(MessageAggregate aggregate, UpdateMessageRelyCommand command, CancellationToken cancellationToken)
    {
        aggregate.UpdateMessageRely(command.Pts);

        return Task.CompletedTask;
    }
}