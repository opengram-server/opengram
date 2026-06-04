namespace MyTelegram.QueryHandlers.MongoDB.Messaging;

public class GetUnreadCountQueryHandler(IQueryOnlyReadModelStore<MessageReadModel> store) : IQueryHandler<GetUnreadCountQuery, int>
{
    public async Task<int> ExecuteQueryAsync(GetUnreadCountQuery query, CancellationToken cancellationToken)
    {
        var count = await store.CountAsync(p =>
            p.OwnerPeerId == query.OwnerUserId && p.ToPeerId == query.ToPeerId && p.MessageId > query.MaxMessageId, cancellationToken);

        return (int)count;
    }
}