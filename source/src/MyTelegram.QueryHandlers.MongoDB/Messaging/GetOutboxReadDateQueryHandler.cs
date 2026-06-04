namespace MyTelegram.QueryHandlers.MongoDB.Messaging;

public class GetOutboxReadDateQueryHandler(IQueryOnlyReadModelStore<ReadingHistoryReadModel> store) : IQueryHandler<GetOutboxReadDateQuery, int>
{
    public async Task<int> ExecuteQueryAsync(GetOutboxReadDateQuery query, CancellationToken cancellationToken)
    {
        return await store.FirstOrDefaultAsync(p =>
            p.ReaderPeerId == query.ToPeer.PeerId &&
            p.TargetPeerId == query.UserId &&
            p.MessageId == query.MessageId,
            p => p.Date, cancellationToken: cancellationToken);
    }
}