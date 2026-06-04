namespace MyTelegram.QueryHandlers.InMemory.Channel;

public class
    GetJoinRequestQueryHandler(IQueryOnlyReadModelStore<JoinChannelRequestReadModel> store) : IQueryHandler<GetJoinRequestQuery, IJoinChannelRequestReadModel?>
{
    public async Task<IJoinChannelRequestReadModel?> ExecuteQueryAsync(GetJoinRequestQuery query, CancellationToken cancellationToken)
    {
        return await store.FirstOrDefaultAsync(p => p.ChannelId == query.ChannelId && p.UserId == query.UserId, cancellationToken);
    }
}