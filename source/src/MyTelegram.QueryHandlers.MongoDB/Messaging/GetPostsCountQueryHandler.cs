namespace MyTelegram.QueryHandlers.MongoDB.Messaging;

public class GetPostsCountQueryHandler(IQueryOnlyReadModelStore<MessageReadModel> store)
    : IQueryHandler<GetPostsCountQuery, int>
{
    public async Task<int> ExecuteQueryAsync(GetPostsCountQuery query, CancellationToken cancellationToken)
    {
        return (int)await store.CountAsync(p => p.PublicPosts && p.Hashtags.Contains(query.Hashtag) && p.Date > query.OffsetRate && p.MessageId > query.OffsetId,
            cancellationToken: cancellationToken);
    }
}