namespace MyTelegram.QueryHandlers.MongoDB.Channel;

public class GetChannelIdListByUserIdQueryHandler
    (IQueryOnlyReadModelStore<ChannelMemberReadModel> store) : IQueryHandler<GetChannelIdListByUserIdQuery, IReadOnlyCollection<long>>
{
    public async Task<IReadOnlyCollection<long>> ExecuteQueryAsync(GetChannelIdListByUserIdQuery query,
        CancellationToken cancellationToken)
    {
        return await store.FindAsync(p => p.UserId == query.UserId, p => p.ChannelId, cancellationToken: cancellationToken);
    }
}
