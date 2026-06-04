namespace MyTelegram.Domain.Aggregates.Story.Events;

public class StoryReactionAddedEvent(long peerId, int storyId, long userId, string reaction) 
    : AggregateEvent<StoryAggregate, StoryId>
{
    public long PeerId { get; } = peerId;
    public int StoryId { get; } = storyId;
    public long UserId { get; } = userId;
    public string Reaction { get; } = reaction;
}
