using MyTelegram.Domain.Aggregates.Story;
using MyTelegram.Domain.Commands.Story;

namespace MyTelegram.Domain.CommandHandlers.Story;

public class EditStoryCommandHandler : CommandHandler<StoryAggregate, StoryId, EditStoryCommand>
{
    public override Task ExecuteAsync(StoryAggregate aggregate, EditStoryCommand command, CancellationToken cancellationToken)
    {
        aggregate.EditStory(command.Media, command.Caption, command.PrivacyRules);
        return Task.CompletedTask;
    }
}
