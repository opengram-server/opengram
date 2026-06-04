namespace MyTelegram.Domain.CommandHandlers.Messaging;

public class ReplyToMessageCommandHandler : CommandHandler<MessageAggregate, MessageId, ReplyToMessageCommand>
{
    public override Task ExecuteAsync(MessageAggregate aggregate,
        ReplyToMessageCommand command,
        CancellationToken cancellationToken)
    {
        aggregate.ReplyToMessage(command.RequestInfo, command.ReplierPeer, command.RepliesPts, command.MessageId);
        return Task.CompletedTask;
    }
}