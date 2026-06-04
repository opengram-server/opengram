namespace MyTelegram.QueryHandlers.MongoDB.Channel;

public class GetChannelUserNameByChannelIdQueryHandler(IQueryOnlyReadModelStore<ChannelReadModel> store) : IQueryHandler<GetChannelUserNameByChannelIdQuery, string?>
{
    public async Task<string?> ExecuteQueryAsync(GetChannelUserNameByChannelIdQuery query,
        CancellationToken cancellationToken)
    {
        return await store.FirstOrDefaultAsync(p => p.ChannelId == query.ChannelId, p => p.UserName, cancellationToken: cancellationToken);
    }
}