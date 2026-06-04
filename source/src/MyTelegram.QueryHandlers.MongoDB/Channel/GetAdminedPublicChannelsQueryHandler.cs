namespace MyTelegram.QueryHandlers.MongoDB.Channel;

public class GetAdminedPublicChannelsQueryHandler(IQueryOnlyReadModelStore<ChannelReadModel> store) : IQueryHandler<GetAdminedPublicChannelsQuery, IReadOnlyCollection<IChannelReadModel>>
{
    public async Task<IReadOnlyCollection<IChannelReadModel>> ExecuteQueryAsync(GetAdminedPublicChannelsQuery query, CancellationToken cancellationToken)
    {
        return await store.FindAsync(p => p.CreatorId == query.UserId && !string.IsNullOrEmpty(p.UserName), cancellationToken: cancellationToken);
    }
}