namespace MyTelegram.QueryHandlers.MongoDB.Messaging;

public class GetTopMessageQueryHandler(IQueryOnlyReadModelStore<MessageReadModel> store)
    : IQueryHandler<GetTopMessageQuery, IMessageReadModel?>
{
    public async Task<IMessageReadModel?> ExecuteQueryAsync(GetTopMessageQuery query, CancellationToken cancellationToken)
    {
        return await store.FirstOrDefaultAsync(
            p => p.OwnerPeerId == query.OwnerPeerId && !query.MessageIds.Contains(p.MessageId),
            p => p,
            sort: new SortOptions<MessageReadModel>(p => p.MessageId, SortType.Descending), cancellationToken: cancellationToken);
    }
}