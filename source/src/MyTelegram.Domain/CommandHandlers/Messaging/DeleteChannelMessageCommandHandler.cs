namespace MyTelegram.Domain.CommandHandlers.Messaging;

public class
    DeleteChannelMessageCommandHandler : CommandHandler<MessageAggregate, MessageId, DeleteChannelMessageCommand>
{
    public override Task ExecuteAsync(MessageAggregate aggregate, DeleteChannelMessageCommand command, CancellationToken cancellationToken)
    {
        aggregate.DeleteChannelMessage(command.RequestInfo);

        return Task.CompletedTask;
    }
}