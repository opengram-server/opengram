namespace MyTelegram.Domain.CommandHandlers.Messaging;

public class AddInboxItemsToOutboxMessageCommandHandler : CommandHandler<MessageAggregate, MessageId, AddInboxItemsToOutboxMessageCommand>
{
    public override Task ExecuteAsync(MessageAggregate aggregate, AddInboxItemsToOutboxMessageCommand command,
        CancellationToken cancellationToken)
    {
        aggregate.AddInboxItemsToOutboxMessage(command.InboxItems);

        return Task.CompletedTask;
    }
}