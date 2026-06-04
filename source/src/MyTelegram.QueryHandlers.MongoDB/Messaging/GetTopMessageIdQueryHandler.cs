namespace MyTelegram.QueryHandlers.MongoDB.Messaging;

public class GetTopMessageIdQueryHandler(IQueryOnlyReadModelStore<MessageReadModel> store) : IQueryHandler<GetTopMessageIdQuery, int>
{
    public Task<int> ExecuteQueryAsync(GetTopMessageIdQuery query, CancellationToken cancellationToken)
    {
        return store.FirstOrDefaultAsync(
            p => p.OwnerPeerId == query.OwnerPeerId && !query.MessageIds.Contains(p.MessageId), p => p.MessageId,
            new SortOptions<MessageReadModel>(p => p.MessageId, SortType.Descending), cancellationToken);
    }
}