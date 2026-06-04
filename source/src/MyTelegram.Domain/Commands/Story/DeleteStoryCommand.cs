using MyTelegram.Domain.Aggregates.Story;

namespace MyTelegram.Domain.Commands.Story;

public class DeleteStoryCommand(StoryId aggregateId) 
    : Command<StoryAggregate, StoryId, IExecutionResult>(aggregateId)
{
}
