namespace MyTelegram.QueryHandlers.InMemory.Messaging;

// ReSharper disable once UnusedMember.Global
public class
    GetReadHistoryMessageQueryHandler(IQueryOnlyReadModelStore<MessageReadModel> store) : IQueryHandler<GetReadHistoryMessageQuery, IMessageReadModel?>
{
    public async Task<IMessageReadModel?> ExecuteQueryAsync(GetReadHistoryMessageQuery query,
        CancellationToken cancellationToken)
    {
        return await store.FirstOrDefaultAsync(
            p => p.OwnerPeerId == query.OwnerPeerId && p.MessageId == query.MessageId && p.ToPeerId == query.ToPeerId,
            cancellationToken: cancellationToken);
    }
}