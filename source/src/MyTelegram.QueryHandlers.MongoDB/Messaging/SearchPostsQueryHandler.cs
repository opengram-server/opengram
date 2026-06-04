namespace MyTelegram.QueryHandlers.MongoDB.Messaging;

public class SearchPostsQueryHandler(IQueryOnlyReadModelStore<MessageReadModel> store)
    : IQueryHandler<SearchPostsQuery, IReadOnlyCollection<IMessageReadModel>>
{
    public async Task<IReadOnlyCollection<IMessageReadModel>> ExecuteQueryAsync(SearchPostsQuery query, CancellationToken cancellationToken)
    {
        return await store.FindAsync(p => p.PublicPosts && p.Hashtags.Contains(query.Hashtag) && p.Date > query.OffsetRate && p.MessageId > query.OffsetId,
            limit: query.Limit,
            cancellationToken: cancellationToken);
    }
}