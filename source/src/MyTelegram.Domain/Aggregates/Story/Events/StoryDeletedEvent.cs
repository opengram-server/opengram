namespace MyTelegram.Domain.Aggregates.Story.Events;

public class StoryDeletedEvent(long peerId, int storyId) 
    : AggregateEvent<StoryAggregate, StoryId>
{
    public long PeerId { get; } = peerId;
    public int StoryId { get; } = storyId;
}
