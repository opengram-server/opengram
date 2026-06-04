namespace MyTelegram.QueryHandlers.MongoDB.Channel;

public class
    GetRecentRequestUserIdListQueryHandler(IQueryOnlyReadModelStore<JoinChannelRequestReadModel> store) : IQueryHandler<GetRecentRequestUserIdListQuery, IReadOnlyCollection<long>>
{
    public async Task<IReadOnlyCollection<long>> ExecuteQueryAsync(GetRecentRequestUserIdListQuery query, CancellationToken cancellationToken)
    {
        return await store.FindAsync(p => p.ChannelId == query.ChannelId && !p.IsJoinRequestProcessed, p => p.UserId,
            limit: query.Limit, sort: new SortOptions<JoinChannelRequestReadModel>(p => p.Date, SortType.Descending), cancellationToken: cancellationToken);
    }
}