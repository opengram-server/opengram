namespace MyTelegram.Domain.Aggregates.Story.Events;

public class StoryPinnedEvent(long peerId, int storyId, bool pinned) 
    : AggregateEvent<StoryAggregate, StoryId>
{
    public long PeerId { get; } = peerId;
    public int StoryId { get; } = storyId;
    public bool Pinned { get; } = pinned;
}
