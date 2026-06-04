namespace MyTelegram.QueryHandlers.InMemory.Messaging;

// ReSharper disable once UnusedMember.Global
public class GetMessageIdListQueryHandler(IQueryOnlyReadModelStore<MessageReadModel> store) : IQueryHandler<GetMessageIdListQuery, List<int>>
{
    public async Task<List<int>> ExecuteQueryAsync(GetMessageIdListQuery query,
        CancellationToken cancellationToken)
    {
        var maxId = query.MaxMessageId;
        if (maxId == 0)
        {
            maxId = int.MaxValue;
        }

        var result= await store
            .FindAsync(p =>
                    p.OwnerPeerId == query.OwnerPeerId && p.ToPeerId == query.ToPeerId && p.MessageId < maxId,
                p => p.MessageId, limit: query.Limit, cancellationToken: cancellationToken);
        return result.ToList();
    }
}
