namespace MyTelegram.QueryHandlers.MongoDB.Messaging;

public class GetMinMessageIdQueryHandler(IQueryOnlyReadModelStore<MessageReadModel> store) : IQueryHandler<GetMinMessageIdQuery, int>
{
    public async Task<int> ExecuteQueryAsync(GetMinMessageIdQuery query, CancellationToken cancellationToken)
    {
        return await store.FirstOrDefaultAsync(p => p.OwnerPeerId == query.SelfUserId || p.ToPeerId == query.SelfUserId,
            p => p.MessageId,
            sort: new SortOptions<MessageReadModel>(p => p.MessageId, SortType.Ascending), cancellationToken: cancellationToken);
    }
}