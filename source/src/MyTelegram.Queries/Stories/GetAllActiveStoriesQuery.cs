namespace MyTelegram.Queries.Stories;

public class GetAllActiveStoriesQuery(long userId, string? state) : IQuery<IReadOnlyList<IStoryReadModel>>
{
    public long UserId { get; } = userId;
    public string? State { get; } = state;
}
