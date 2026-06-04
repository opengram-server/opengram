namespace MyTelegram.QueryHandlers.MongoDB.Messaging;

// ReSharper disable once UnusedMember.Global
public class GetMessageByIdQueryHandler(IQueryOnlyReadModelStore<MessageReadModel> store) : IQueryHandler<GetMessageByIdQuery, IMessageReadModel?>
{
    public async Task<IMessageReadModel?> ExecuteQueryAsync(GetMessageByIdQuery query,
        CancellationToken cancellationToken)
    {
        return await store.FirstOrDefaultAsync(p => p.Id == query.Id, cancellationToken);
    }
}