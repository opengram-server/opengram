namespace MyTelegram.QueryHandlers.MongoDB.Messaging;

public class GetMessageItemListToBeDeletedQueryHandler(IQueryOnlyReadModelStore<MessageReadModel> store)
    : IQueryHandler<GetMessageItemListToBeDeletedQuery, IReadOnlyCollection<MessageItemToBeDeleted>>
{
    public async Task<IReadOnlyCollection<MessageItemToBeDeleted>> ExecuteQueryAsync(GetMessageItemListToBeDeletedQuery query, CancellationToken cancellationToken)
    {
        if (!query.Revoke)
        {
            return await store.FindAsync(
                p => p.OwnerPeerId == query.OwnerPeerId && query.MessageIds.Contains(p.MessageId),
                p => new MessageItemToBeDeleted(p.OwnerPeerId, p.MessageId, p.ToPeerType, p.ToPeerId),
                cancellationToken: cancellationToken);
        }

        var batchIds =
                (await store.FindAsync(
                    p => p.OwnerPeerId == query.OwnerPeerId && query.MessageIds.Contains(p.MessageId),
                    p => p.BatchId, cancellationToken: cancellationToken));

        if (batchIds.Count == 0)
        {
            return [];
        }

        return await store.FindAsync(p => batchIds.Contains(p.BatchId),
                p => new MessageItemToBeDeleted(p.OwnerPeerId, p.MessageId, p.ToPeerType, p.ToPeerId),
                cancellationToken: cancellationToken);
    }
}