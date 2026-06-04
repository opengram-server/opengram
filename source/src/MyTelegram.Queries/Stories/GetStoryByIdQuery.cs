namespace MyTelegram.Queries.Stories;

public class GetStoryByIdQuery(long peerId, int storyId) : IQuery<IStoryReadModel?>
{
    public long PeerId { get; } = peerId;
    public int StoryId { get; } = storyId;
}
