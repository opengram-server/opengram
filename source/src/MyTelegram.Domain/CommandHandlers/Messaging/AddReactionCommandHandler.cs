using MyTelegram.Domain.Aggregates.Messaging;
using MyTelegram.Domain.Commands.Messaging;

namespace MyTelegram.Domain.CommandHandlers.Messaging;

public class AddReactionCommandHandler : CommandHandler<MessageAggregate, MessageId, AddReactionCommand>
{
    public override Task ExecuteAsync(MessageAggregate aggregate,
        AddReactionCommand command,
        CancellationToken cancellationToken)
    {
        aggregate.AddReaction(
            command.RequestInfo,
            command.SenderUserId,
            command.SenderPeer,
            command.Reaction,
            command.Big,
            command.AddToRecent,
            command.Date,
            command.Count);

        return Task.CompletedTask;
    }
}
