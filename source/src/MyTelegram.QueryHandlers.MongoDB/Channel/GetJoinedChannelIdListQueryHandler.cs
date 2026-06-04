namespace MyTelegram.QueryHandlers.MongoDB.Channel;

public class
    GetJoinedChannelIdListQueryHandler(IQueryOnlyReadModelStore<ChannelMemberReadModel> store) : IQueryHandler<GetJoinedChannelIdListQuery, IReadOnlyCollection<long>>
{
    public async Task<IReadOnlyCollection<long>> ExecuteQueryAsync(GetJoinedChannelIdListQuery query,
        CancellationToken cancellationToken)
    {
        return await store.FindAsync(p => p.UserId == query.MemberUserId && !p.Left && !p.Kicked && query.ChannelIdList.Contains(p.ChannelId),
            p => p.ChannelId, cancellationToken: cancellationToken);
    }
}