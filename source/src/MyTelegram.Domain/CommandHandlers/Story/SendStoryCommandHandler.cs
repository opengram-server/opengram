using MyTelegram.Domain.Aggregates.Story;
using MyTelegram.Domain.Commands.Story;

namespace MyTelegram.Domain.CommandHandlers.Story;

public class SendStoryCommandHandler : CommandHandler<StoryAggregate, StoryId, SendStoryCommand>
{
    public override Task ExecuteAsync(StoryAggregate aggregate, SendStoryCommand command, CancellationToken cancellationToken)
    {
        aggregate.CreateStory(
            command.PeerId,
            command.StoryId,
            command.Media,
            command.Caption,
            command.PrivacyRules,
            command.Date,
            command.ExpireDate,
            command.Pinned,
            command.NoForwards,
            command.IsPublic);

        return Task.CompletedTask;
    }
}
