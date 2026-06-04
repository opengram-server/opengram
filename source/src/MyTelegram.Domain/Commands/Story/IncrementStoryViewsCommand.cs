using MyTelegram.Domain.Aggregates.Story;

namespace MyTelegram.Domain.Commands.Story;

public class IncrementStoryViewsCommand(StoryId aggregateId, int viewsCount) 
    : Command<StoryAggregate, StoryId, IExecutionResult>(aggregateId)
{
    public int ViewsCount { get; } = viewsCount;
}
