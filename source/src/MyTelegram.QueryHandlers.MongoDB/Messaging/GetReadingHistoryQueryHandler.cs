namespace MyTelegram.QueryHandlers.MongoDB.Messaging;

// ReSharper disable once UnusedMember.Global
public class
    GetReadingHistoryQueryHandler(IQueryOnlyReadModelStore<ReadingHistoryReadModel> store) : IQueryHandler<GetReadingHistoryQuery, IReadOnlyCollection<long>>
{
    public async Task<IReadOnlyCollection<long>> ExecuteQueryAsync(GetReadingHistoryQuery query,
        CancellationToken cancellationToken)
    {
        return await store
            .FindAsync(p => p.TargetPeerId == query.TargetPeerId && p.MessageId == query.MessageId,
                p => p.TargetPeerId,
                0, 200, cancellationToken: cancellationToken);
    }
}