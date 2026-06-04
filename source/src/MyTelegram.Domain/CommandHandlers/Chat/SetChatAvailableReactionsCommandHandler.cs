using MyTelegram.Domain.Aggregates.Chat;
using MyTelegram.Domain.Commands.Chat;

namespace MyTelegram.Domain.CommandHandlers.Chat;

public class SetChatAvailableReactionsCommandHandler : CommandHandler<ChatAggregate, ChatId, SetChatAvailableReactionsCommand>
{
    public override Task ExecuteAsync(ChatAggregate aggregate,
        SetChatAvailableReactionsCommand command,
        CancellationToken cancellationToken)
    {
        aggregate.SetAvailableReactions(
            command.RequestInfo,
            command.ReactionType,
            command.AllowCustomReaction,
            command.AvailableReactions,
            command.ReactionsLimit);

        return Task.CompletedTask;
    }
}
