using MyTelegram.Domain.Aggregates.Messaging;
using MyTelegram.Domain.Commands.Messaging;

namespace MyTelegram.Domain.CommandHandlers.Messaging;

public class RemoveReactionCommandHandler : CommandHandler<MessageAggregate, MessageId, RemoveReactionCommand>
{
    public override Task ExecuteAsync(MessageAggregate aggregate,
        RemoveReactionCommand command,
        CancellationToken cancellationToken)
    {
        aggregate.RemoveReaction(
            command.RequestInfo,
            command.SenderUserId,
            command.Reaction);

        return Task.CompletedTask;
    }
}
