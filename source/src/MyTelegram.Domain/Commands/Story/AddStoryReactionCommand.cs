using MyTelegram.Domain.Aggregates.Story;

namespace MyTelegram.Domain.Commands.Story;

public class AddStoryReactionCommand(
    StoryId aggregateId,
    RequestInfo requestInfo,
    long userId,
    string reaction)
    : RequestCommand2<StoryAggregate, StoryId, IExecutionResult>(aggregateId, requestInfo)
{
    public long UserId { get; } = userId;
    public string Reaction { get; } = reaction;
}
