namespace MyTelegram.QueryHandlers.MongoDB.Channel;

public class GetPendingRequestsCountQueryHandler(IQueryOnlyReadModelStore<JoinChannelRequestReadModel> store) : IQueryHandler<GetPendingRequestsCountQuery, int>
{
    public async Task<int> ExecuteQueryAsync(GetPendingRequestsCountQuery query, CancellationToken cancellationToken)
    {
        return (int)await store.CountAsync(p => p.ChannelId == query.ChannelId && !p.IsJoinRequestProcessed, cancellationToken);
    }
}