using MyTelegram.Domain.Aggregates.Story;
using MyTelegram.Domain.Commands.Story;

namespace MyTelegram.Domain.CommandHandlers.Story;

public class DeleteStoryCommandHandler : CommandHandler<StoryAggregate, StoryId, DeleteStoryCommand>
{
    public override Task ExecuteAsync(StoryAggregate aggregate, DeleteStoryCommand command, CancellationToken cancellationToken)
    {
        aggregate.DeleteStory();
        return Task.CompletedTask;
    }
}
