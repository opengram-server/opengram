using MyTelegram.Domain.Aggregates.Story;

namespace MyTelegram.Domain.Commands.Story;

public class ToggleStoryPinnedCommand(
    StoryId aggregateId,
    RequestInfo requestInfo,
    bool pinned)
    : RequestCommand2<StoryAggregate, StoryId, IExecutionResult>(aggregateId, requestInfo)
{
    public bool Pinned { get; } = pinned;
}
