namespace MyTelegram.QueryHandlers.InMemory.Messaging;

public class GetCommentsMessageIdListQueryHandler
    (IQueryOnlyReadModelStore<MessageReadModel> store) : IQueryHandler<GetCommentsMessageIdListQuery,
        IReadOnlyCollection<int>>
{
    public async Task<IReadOnlyCollection<int>> ExecuteQueryAsync(GetCommentsMessageIdListQuery query, CancellationToken cancellationToken)
    {
        return await store.FindAsync(p =>
            p.PostChannelId == query.ChannelId && query.MessageIds.Contains(p.PostMessageId ?? 0), p => p.MessageId, cancellationToken: cancellationToken);
    }
}