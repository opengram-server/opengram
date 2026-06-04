namespace MyTelegram.QueryHandlers.MongoDB.Messaging;

public class GetMessageByPeerIdAndMessageIdQueryHandler
    (IQueryOnlyReadModelStore<MessageReadModel> store) : IQueryHandler<GetMessageByPeerIdAndMessageIdQuery,
        IMessageReadModel?>
{
    public async Task<IMessageReadModel?> ExecuteQueryAsync(GetMessageByPeerIdAndMessageIdQuery query, CancellationToken cancellationToken)
    {
        return await store.FirstOrDefaultAsync(
            p => p.OwnerPeerId == query.OwnerPeerId && p.MessageId == query.MessageId, cancellationToken);
    }
}