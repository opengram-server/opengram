namespace MyTelegram.QueryHandlers.MongoDB.Channel;

public class GetKickedChannelMembersQueryHandler(IQueryOnlyReadModelStore<ChannelMemberReadModel> store) : IQueryHandler<GetKickedChannelMembersQuery,
    IReadOnlyCollection<IChannelMemberReadModel>>
{
    public async Task<IReadOnlyCollection<IChannelMemberReadModel>> ExecuteQueryAsync(
        GetKickedChannelMembersQuery query,
        CancellationToken cancellationToken)
    {
        return await store.FindAsync(p => p.ChannelId == query.ChannelId && p.Kicked, skip: query.Offset, limit: query.Limit, cancellationToken: cancellationToken);
    }
}
