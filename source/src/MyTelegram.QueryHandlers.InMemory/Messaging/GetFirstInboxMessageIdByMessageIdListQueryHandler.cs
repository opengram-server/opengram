namespace MyTelegram.QueryHandlers.InMemory.Messaging;

public class
    GetFirstInboxMessageIdByMessageIdListQueryHandler(IQueryOnlyReadModelStore<MessageReadModel> store) : IQueryHandler<GetFirstInboxMessageIdByMessageIdListQuery, int?>
{
    public async Task<int?> ExecuteQueryAsync(GetFirstInboxMessageIdByMessageIdListQuery query, CancellationToken cancellationToken)
    {
        return await store.FirstOrDefaultAsync(p =>
            p.OwnerPeerId == query.ChannelId && !p.Out && query.MessageIds.Contains(p.MessageId), p => p.MessageId, cancellationToken: cancellationToken);
    }
}