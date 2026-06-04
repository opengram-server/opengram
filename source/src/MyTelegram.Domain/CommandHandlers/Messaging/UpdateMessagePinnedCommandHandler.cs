namespace MyTelegram.Domain.CommandHandlers.Messaging;

public class UpdateMessagePinnedCommandHandler : CommandHandler<MessageAggregate, MessageId, UpdateMessagePinnedCommand>
{
    public override Task ExecuteAsync(MessageAggregate aggregate, UpdateMessagePinnedCommand command, CancellationToken cancellationToken)
    {
        aggregate.UpdateMessagePinned(command.RequestInfo, command.Pinned);

        return Task.CompletedTask;
    }
}