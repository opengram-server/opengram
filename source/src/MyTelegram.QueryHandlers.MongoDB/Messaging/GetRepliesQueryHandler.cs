namespace MyTelegram.QueryHandlers.MongoDB.Messaging;

public class GetRepliesQueryHandler(IQueryOnlyReadModelStore<ReplyReadModel> store) : IQueryHandler<GetRepliesQuery, IReadOnlyCollection<IReplyReadModel>>
{
    public async Task<IReadOnlyCollection<IReplyReadModel>> ExecuteQueryAsync(GetRepliesQuery query,
        CancellationToken cancellationToken)
    {
        return await store
            .FindAsync(p => p.ChannelId == query.ChannelId && query.MessageIds.Contains(p.MessageId), cancellationToken: cancellationToken);
    }
}