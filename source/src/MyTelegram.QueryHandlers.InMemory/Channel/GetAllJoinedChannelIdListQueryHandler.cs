namespace MyTelegram.QueryHandlers.InMemory.Channel;

public class
    GetAllJoinedChannelIdListQueryHandler(IQueryOnlyReadModelStore<ChannelMemberReadModel> store) : IQueryHandler<GetAllJoinedChannelIdListQuery, IReadOnlyCollection<long>>
{
    public async Task<IReadOnlyCollection<long>> ExecuteQueryAsync(GetAllJoinedChannelIdListQuery query,
        CancellationToken cancellationToken)
    {
        return await store.FindAsync(p => p.UserId == query.SelfUserId,
            p => p.ChannelId, cancellationToken: cancellationToken);
    }
}