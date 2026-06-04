namespace MyTelegram.QueryHandlers.InMemory.Channel;

public class GetChannelTopMessageIdQueryHandler(IQueryOnlyReadModelStore<ChannelReadModel> store) : IQueryHandler<GetChannelTopMessageIdQuery, int?>
{
    public async Task<int?> ExecuteQueryAsync(GetChannelTopMessageIdQuery query, CancellationToken cancellationToken)
    {
        return await store.FirstOrDefaultAsync(p => p.ChannelId == query.ChannelId, p => p.TopMessageId, cancellationToken: cancellationToken);
    }
}