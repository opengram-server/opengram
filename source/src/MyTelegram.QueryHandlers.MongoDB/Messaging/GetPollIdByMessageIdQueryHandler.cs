namespace MyTelegram.QueryHandlers.MongoDB.Messaging;

public class GetPollIdByMessageIdQueryHandler(IQueryOnlyReadModelStore<MessageReadModel> store) : IQueryHandler<GetPollIdByMessageIdQuery, long?>
{
    public async Task<long?> ExecuteQueryAsync(GetPollIdByMessageIdQuery query,
        CancellationToken cancellationToken)
    {
        return await store.FirstOrDefaultAsync(p => p.ToPeerId == query.PeerId && p.MessageId == query.MessageId,
            p => p.PollId, cancellationToken: cancellationToken);
    }
}
