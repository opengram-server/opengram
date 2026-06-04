namespace MyTelegram.QueryHandlers.MongoDB.Messaging;

public class GetMaxMessageByChatIdQueryHandler(IQueryOnlyReadModelStore<MessageReadModel> store) : IQueryHandler<GetMaxMessageByChatIdQuery, int>
{
    public async Task<int> ExecuteQueryAsync(GetMaxMessageByChatIdQuery query,
        CancellationToken cancellationToken)
    {
        return await store.FirstOrDefaultAsync(p => p.OwnerPeerId == query.SelfUserId && p.ToPeerId == query.ChatId,
            p => p.MessageId,
            sort: new SortOptions<MessageReadModel>(p => p.MessageId, SortType.Descending)
            , cancellationToken);
    }
}