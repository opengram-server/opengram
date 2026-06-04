namespace MyTelegram.QueryHandlers.MongoDB.Channel;

public class GetChannelMemberIdListQueryHandler
    (IQueryOnlyReadModelStore<ChannelMemberReadModel> store) : IQueryHandler<GetChannelMemberIdListQuery, IReadOnlyCollection<long>>
{
    public async Task<IReadOnlyCollection<long>> ExecuteQueryAsync(GetChannelMemberIdListQuery query,
        CancellationToken cancellationToken)
    {
        return await store.FindAsync(p => p.ChannelId == query.ChannelId && !p.Left && !p.Kicked && query.MemberUserIds.Contains(p.UserId), p => p.UserId, cancellationToken: cancellationToken);
    }
}