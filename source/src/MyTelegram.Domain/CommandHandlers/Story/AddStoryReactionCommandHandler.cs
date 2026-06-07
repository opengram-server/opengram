using MyTelegram.Domain.Aggregates.Story;
using MyTelegram.Domain.Commands.Story;

namespace MyTelegram.Domain.CommandHandlers.Story;

public class AddStoryReactionCommandHandler : CommandHandler<StoryAggregate, StoryId, AddStoryReactionCommand>
{
    public override Task ExecuteAsync(StoryAggregate aggregate, AddStoryReactionCommand command, CancellationToken cancellationToken)
    {
        aggregate.AddReaction(command.UserId, command.Reaction);
        return Task.CompletedTask;
    }
}
