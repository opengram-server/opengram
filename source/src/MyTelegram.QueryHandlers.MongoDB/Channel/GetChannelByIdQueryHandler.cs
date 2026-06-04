namespace MyTelegram.QueryHandlers.MongoDB.Channel;

public class GetChannelByIdQueryHandler(IQueryOnlyReadModelStore<ChannelReadModel> store) : IQueryHandler<GetChannelByIdQuery, IChannelReadModel?>
{
    public async Task<IChannelReadModel?> ExecuteQueryAsync(GetChannelByIdQuery query,
        CancellationToken cancellationToken)
    {
        return await store.FirstOrDefaultAsync(p => p.ChannelId == query.ChannelId, cancellationToken);
    }
}