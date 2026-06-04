namespace MyTelegram.QueryHandlers.InMemory.Channel;

public class GetAdminedChannelIdsQueryHandler(IQueryOnlyReadModelStore<ChannelReadModel> store) : IQueryHandler<GetAdminedPublicChannelIdsQuery, IReadOnlyCollection<long>>
{
    public async Task<IReadOnlyCollection<long>> ExecuteQueryAsync(GetAdminedPublicChannelIdsQuery query, CancellationToken cancellationToken)
    {
        return await store.FindAsync(p => p.CreatorId == query.UserId && !string.IsNullOrEmpty(p.UserName), createResult: p => p.ChannelId, cancellationToken: cancellationToken);
    }
}