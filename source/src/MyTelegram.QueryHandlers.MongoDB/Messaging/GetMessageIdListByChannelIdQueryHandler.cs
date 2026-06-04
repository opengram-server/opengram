namespace MyTelegram.QueryHandlers.MongoDB.Messaging;

public class
    GetMessageIdListByChannelIdQueryHandler(IQueryOnlyReadModelStore<MessageReadModel> store) : IQueryHandler<GetMessageIdListByChannelIdQuery, IReadOnlyCollection<int>>
{
    public async Task<IReadOnlyCollection<int>> ExecuteQueryAsync(GetMessageIdListByChannelIdQuery query,
        CancellationToken cancellationToken)
    {
        return await store.FindAsync(p => p.OwnerPeerId == query.ChannelId,
                p => p.MessageId, limit: query.Limit, cancellationToken: cancellationToken);
    }
}