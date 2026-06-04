namespace MyTelegram.QueryHandlers.MongoDB.Messaging;

public class
    GetMessageIdListByUserIdQueryHandler(IQueryOnlyReadModelStore<MessageReadModel> store) : IQueryHandler<GetMessageIdListByUserIdQuery, IReadOnlyCollection<int>>
{
    public async Task<IReadOnlyCollection<int>> ExecuteQueryAsync(GetMessageIdListByUserIdQuery query,
        CancellationToken cancellationToken)
    {
        Expression<Func<MessageReadModel, bool>> predicate = x => x.OwnerPeerId == query.ChannelId;
        predicate = predicate.WhereIf(query.SenderUserId != 0, p => p.SenderPeerId == query.SenderUserId);

        return await store.FindAsync(predicate,
               p => p.MessageId,
               limit: query.Limit, cancellationToken: cancellationToken);
    }
}