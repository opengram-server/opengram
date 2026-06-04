namespace MyTelegram.Domain.Events.Temp;

public class TogglePinnedToTopStartedEvent(RequestInfo requestInfo, Peer peer, List<int> storyIds) : RequestAggregateEvent2<TempAggregate, TempId>(requestInfo)
{
    public Peer Peer { get; } = peer;
    public List<int> StoryIds { get; } = storyIds;
}