namespace MyTelegram.QueryHandlers.InMemory.Channel;

public class GetLeftChannelIdsQueryHandler
    (IQueryOnlyReadModelStore<ChannelMemberReadModel> store) : IQueryHandler<GetLeftChannelIdsQuery, IReadOnlyCollection<long>>
{
    public async Task<IReadOnlyCollection<long>> ExecuteQueryAsync(GetLeftChannelIdsQuery query,
        CancellationToken cancellationToken)
    {
        Expression<Func<ChannelMemberReadModel, bool>> predicate = p => p.UserId == query.UserId && p.Left;
        predicate = predicate.WhereIf(query.ChannelIds?.Count > 0, p => query.ChannelIds!.Contains(p.ChannelId))
                .WhereIf(query.OffsetChannelId > 0, p => p.ChannelId > query.OffsetChannelId)
            ;
        SortOptions<ChannelMemberReadModel>? sort = null;
        if (query.OffsetChannelId > 0)
        {
            sort = new SortOptions<ChannelMemberReadModel>(p => p.ChannelId, SortType.Ascending);
        }

        return await store.FindAsync(predicate, p => p.ChannelId, limit: query.Limit, sort: sort, cancellationToken: cancellationToken);
    }
}