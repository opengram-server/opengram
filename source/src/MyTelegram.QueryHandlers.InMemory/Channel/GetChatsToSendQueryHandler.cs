namespace MyTelegram.QueryHandlers.InMemory.Channel;

public class GetChatsToSendQueryHandler(IQueryOnlyReadModelStore<ChannelReadModel> store,
    IQueryOnlyReadModelStore<ChatAdminReadModel> chatAdminStore
) :
    IQueryHandler<GetChatsToSendQuery, IReadOnlyCollection<long>>
{
    public async Task<IReadOnlyCollection<long>> ExecuteQueryAsync(GetChatsToSendQuery query, CancellationToken cancellationToken)
    {
        var adminedChannelIds = await chatAdminStore.FindAsync(p => p.UserId == query.UserId && p.AdminRights.PostStories, createResult: p => p.PeerId, cancellationToken: cancellationToken) ?? [];
        return await store.FindAsync(p => p.CreatorId == query.UserId || adminedChannelIds.Contains(p.ChannelId), createResult: p => p.ChannelId, cancellationToken: cancellationToken);
    }
}