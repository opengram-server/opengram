namespace MyTelegram.Domain.Aggregates.Story;

public class StoryId : Identity<StoryId>
{
    public StoryId(string value) : base(value)
    {
    }

    public static StoryId Create(long peerId, int storyId)
    {
        return NewDeterministic(GuidFactories.Deterministic.Namespaces.Commands, $"story-{peerId}-{storyId}");
    }
}
