namespace MyTelegram.QueryHandlers.MongoDB.Messaging;

public class
    GetMessagesByUserQueryHandler(IQueryOnlyReadModelStore<MessageReadModel> store) : IQueryHandler<GetMessagesByUserIdQuery,
        IReadOnlyCollection<IMessageReadModel>>
{
    public async Task<IReadOnlyCollection<IMessageReadModel>> ExecuteQueryAsync(GetMessagesByUserIdQuery query,
        CancellationToken cancellationToken)
    {
        return await store.FindAsync(p => p.OwnerPeerId == query.OwnerPeerId && p.ToPeerId == query.ToPeerId,
                cancellationToken: cancellationToken);
    }
}