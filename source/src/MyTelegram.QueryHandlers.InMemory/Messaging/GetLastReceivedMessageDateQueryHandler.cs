namespace MyTelegram.QueryHandlers.InMemory.Messaging;

public class GetLastReceivedMessageDateQueryHandler(IQueryOnlyReadModelStore<MessageReadModel> store)
    : IQueryHandler<GetLastReceivedMessageDateQuery, int?>
{
    public async Task<int?> ExecuteQueryAsync(GetLastReceivedMessageDateQuery query, CancellationToken cancellationToken)
    {
        return await store.FirstOrDefaultAsync(
            p => p.OwnerPeerId == query.OwnerUserId && p.ToPeerId == query.SenderUserId && p.Date != query.LatestReceiveMessageDate,
            p => p.Date,
            sort: new SortOptions<MessageReadModel>(p => p.MessageId, SortType.Descending), cancellationToken: cancellationToken);
    }
}