namespace MyTelegram.Domain.Aggregates.Story.Events;

public class StoryViewIncrementedEvent(long peerId, int storyId, int viewsCount) 
    : AggregateEvent<StoryAggregate, StoryId>
{
    public long PeerId { get; } = peerId;
    public int StoryId { get; } = storyId;
    public int ViewsCount { get; } = viewsCount;
}
