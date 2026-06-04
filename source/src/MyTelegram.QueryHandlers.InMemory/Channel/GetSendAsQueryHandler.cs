namespace MyTelegram.QueryHandlers.InMemory.Channel;

public class GetSendAsQueryHandler
    (IQueryOnlyReadModelStore<ChannelReadModel> store) : IQueryHandler<GetSendAsQuery, IReadOnlyCollection<IChannelReadModel>>
{
    public async Task<IReadOnlyCollection<IChannelReadModel>> ExecuteQueryAsync(GetSendAsQuery query, CancellationToken cancellationToken)
    {
        return await store.FindAsync(p => p.CreatorId == query.SelfUserId && p.Broadcast && !string.IsNullOrEmpty(p.UserName), cancellationToken: cancellationToken);
    }
}