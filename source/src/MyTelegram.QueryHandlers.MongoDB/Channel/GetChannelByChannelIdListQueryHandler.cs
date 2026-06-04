namespace MyTelegram.QueryHandlers.MongoDB.Channel;

public class
    GetChannelByChannelIdListQueryHandler(IQueryOnlyReadModelStore<ChannelReadModel> store) : IQueryHandler<GetChannelByChannelIdListQuery,
        IReadOnlyCollection<IChannelReadModel>>
{
    public async Task<IReadOnlyCollection<IChannelReadModel>> ExecuteQueryAsync(GetChannelByChannelIdListQuery query,
        CancellationToken cancellationToken)
    {
        if (query.ChannelIdList.Count == 0)
        {
            return new List<ChannelReadModel>();
        }

        return await store.FindAsync(p => query.ChannelIdList.Contains(p.ChannelId) && !p.IsDeleted, cancellationToken: cancellationToken);
    }
}
