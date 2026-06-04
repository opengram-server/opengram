using MyTelegram.Domain.Aggregates.Channel;
using MyTelegram.Domain.Commands.Channel;

namespace MyTelegram.Domain.CommandHandlers.Channel;

public class SetChannelAvailableReactionsCommandHandler : CommandHandler<ChannelAggregate, ChannelId, SetChannelAvailableReactionsCommand>
{
    public override Task ExecuteAsync(ChannelAggregate aggregate,
        SetChannelAvailableReactionsCommand command,
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
