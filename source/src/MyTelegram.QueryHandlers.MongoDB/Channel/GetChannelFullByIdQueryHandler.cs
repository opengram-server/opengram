namespace MyTelegram.QueryHandlers.MongoDB.Channel;

public class GetChannelFullByIdQueryHandler(IQueryOnlyReadModelStore<ChannelFullReadModel> store) : IQueryHandler<GetChannelFullByIdQuery, IChannelFullReadModel?>
{
    public async Task<IChannelFullReadModel?> ExecuteQueryAsync(GetChannelFullByIdQuery query,
        CancellationToken cancellationToken)
    {
        return await store.FirstOrDefaultAsync(p => p.ChannelId == query.ChannelId, cancellationToken);
    }
}
