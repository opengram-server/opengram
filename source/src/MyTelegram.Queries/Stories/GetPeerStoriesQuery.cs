namespace MyTelegram.Queries.Stories;

public class GetPeerStoriesQuery(long userId, long peerId) : IQuery<IReadOnlyList<IStoryReadModel>>
{
    public long UserId { get; } = userId;
    public long PeerId { get; } = peerId;
}
