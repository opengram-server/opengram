using MyTelegram.Domain.Aggregates.Story;
using MyTelegram.Domain.Commands.Story;

namespace MyTelegram.Domain.CommandHandlers.Story;

public class ToggleStoryPinnedCommandHandler : CommandHandler<StoryAggregate, StoryId, ToggleStoryPinnedCommand>
{
    public override Task ExecuteAsync(StoryAggregate aggregate, ToggleStoryPinnedCommand command, CancellationToken cancellationToken)
    {
        aggregate.TogglePinned(command.Pinned);
        return Task.CompletedTask;
    }
}
