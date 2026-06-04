namespace MyTelegram.QueryHandlers.MongoDB.Channel;

public class
    GetChannelIdListByMemberUserIdQueryHandler(IQueryOnlyReadModelStore<ChannelMemberReadModel> store) : IQueryHandler<GetChannelIdListByMemberUserIdQuery,
        IReadOnlyCollection<long>>
{
    public async Task<IReadOnlyCollection<long>> ExecuteQueryAsync(GetChannelIdListByMemberUserIdQuery query,
        CancellationToken cancellationToken)
    {
        return await store.FindAsync(p => p.UserId == query.MemberUserId, p => p.ChannelId, limit: 100, cancellationToken: cancellationToken);
    }
}
