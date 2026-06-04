namespace MyTelegram.QueryHandlers.InMemory.Channel;

public class GetSendAsPeerIdQueryHandler(IQueryOnlyReadModelStore<ChannelReadModel> store)
    : IQueryHandler<GetSendAsPeerIdQuery, long?>
{
    public async Task<long?> ExecuteQueryAsync(GetSendAsPeerIdQuery query, CancellationToken cancellationToken)
    {
        return await store.
            FirstOrDefaultAsync(p => (p.LinkedChatId == query.LinkedChannelId || p.ChannelId == query.LinkedChannelId) && p.CreatorId == query.CreatorUserId, p => p.ChannelId, cancellationToken: cancellationToken);
    }
}