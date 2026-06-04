namespace MyTelegram.Domain.Aggregates.Story.Events;

public class StoryEditedEvent(long peerId, int storyId, byte[]? media, string? caption, List<long>? privacyRules) 
    : AggregateEvent<StoryAggregate, StoryId>
{
    public long PeerId { get; } = peerId;
    public int StoryId { get; } = storyId;
    public byte[]? Media { get; } = media;
    public string? Caption { get; } = caption;
    public List<long>? PrivacyRules { get; } = privacyRules;
}
