namespace MyTelegram.QueryHandlers.MongoDB.Channel;

public class GetLinkedChannelIdQueryHandler(IQueryOnlyReadModelStore<ChannelReadModel> store) : IQueryHandler<GetLinkedChannelIdQuery, long?>
{
    public async Task<long?> ExecuteQueryAsync(GetLinkedChannelIdQuery query,
        CancellationToken cancellationToken)
    {
        return await store.FirstOrDefaultAsync(p => p.ChannelId == query.ChannelId && p.Broadcast, p => p.LinkedChatId, cancellationToken: cancellationToken);
    }
}