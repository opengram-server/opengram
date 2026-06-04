using MyTelegram.Domain.Aggregates.Story;
using MyTelegram.Domain.Commands.Story;

namespace MyTelegram.Domain.CommandHandlers.Story;

public class IncrementStoryViewsCommandHandler : CommandHandler<StoryAggregate, StoryId, IncrementStoryViewsCommand>
{
    public override Task ExecuteAsync(StoryAggregate aggregate, IncrementStoryViewsCommand command, CancellationToken cancellationToken)
    {
        aggregate.IncrementViews(command.ViewsCount);
        return Task.CompletedTask;
    }
}
