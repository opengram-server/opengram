namespace MyTelegram.QueryHandlers.MongoDB.Channel;

public class GetChannelIdsByKeywordQueryHandler(IQueryOnlyReadModelStore<ChannelReadModel> store)
    : IQueryHandler<GetChannelIdsByKeywordQuery, IReadOnlyCollection<long>>
{
    public async Task<IReadOnlyCollection<long>> ExecuteQueryAsync(GetChannelIdsByKeywordQuery query,
        CancellationToken cancellationToken)
    {
        return await store.FindAsync(p => p.Title.Contains(query.Keyword) &&
                                          (!string.IsNullOrEmpty(p.UserName) || p.CreatorId == query.UserId),
            createResult: p => p.ChannelId,
            limit: query.Limit, cancellationToken: cancellationToken);
    }
}