namespace MyTelegram.Domain.CommandHandlers.Messaging;

public class PinChannelMessageCommandHandler : CommandHandler<MessageAggregate, MessageId, PinChannelMessageCommand>
{
    public override Task ExecuteAsync(MessageAggregate aggregate, PinChannelMessageCommand command, CancellationToken cancellationToken)
    {
        aggregate.PinChannelMessage(command.RequestInfo);

        return Task.CompletedTask;
    }
}