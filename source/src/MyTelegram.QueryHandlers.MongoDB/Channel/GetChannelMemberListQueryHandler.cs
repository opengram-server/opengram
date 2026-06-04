namespace MyTelegram.QueryHandlers.MongoDB.Channel;

public class GetChannelMemberListQueryHandler
    (IQueryOnlyReadModelStore<ChannelMemberReadModel> store) : IQueryHandler<GetChannelMemberListQuery, IReadOnlyCollection<long>>
{
    public async Task<IReadOnlyCollection<long>> ExecuteQueryAsync(GetChannelMemberListQuery query,
        CancellationToken cancellationToken)
    {
        return await store.FindAsync(p => p.ChannelId == query.ChannelId && !p.Left && !p.Kicked, p => p.UserId, cancellationToken: cancellationToken);
    }
}