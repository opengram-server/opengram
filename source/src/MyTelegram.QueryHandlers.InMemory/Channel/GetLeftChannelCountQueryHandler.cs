namespace MyTelegram.QueryHandlers.InMemory.Channel;

public class GetLeftChannelCountQueryHandler
    (IQueryOnlyReadModelStore<ChannelMemberReadModel> store) : IQueryHandler<GetLeftChannelCountQuery, int>
{
    public async Task<int> ExecuteQueryAsync(GetLeftChannelCountQuery query,
        CancellationToken cancellationToken)
    {
        return (int)await store.CountAsync(p => p.UserId == query.UserId && p.Left, cancellationToken);
    }
}