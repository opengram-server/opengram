namespace MyTelegram.QueryHandlers.MongoDB.Channel;

public class GetChannelMemberByUserIdQueryHandler
    (IQueryOnlyReadModelStore<ChannelMemberReadModel> store) : IQueryHandler<GetChannelMemberByUserIdQuery, IChannelMemberReadModel?>
{
    public async Task<IChannelMemberReadModel?> ExecuteQueryAsync(GetChannelMemberByUserIdQuery query,
        CancellationToken cancellationToken)
    {
        return await store.FirstOrDefaultAsync(p => p.ChannelId == query.ChannelId && p.UserId == query.UserId, cancellationToken);
    }
}