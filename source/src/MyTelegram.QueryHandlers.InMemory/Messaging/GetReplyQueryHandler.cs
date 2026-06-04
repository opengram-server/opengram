namespace MyTelegram.QueryHandlers.InMemory.Messaging;

public class GetReplyQueryHandler(IQueryOnlyReadModelStore<ReplyReadModel> store) : IQueryHandler<GetReplyQuery, IReplyReadModel?>
{
    public async Task<IReplyReadModel?> ExecuteQueryAsync(GetReplyQuery query,
        CancellationToken cancellationToken)
    {
        return await store.FirstOrDefaultAsync(p =>
                p.ChannelId == query.ChannelId && p.MessageId == query.SavedFromMsgId,
            cancellationToken: cancellationToken);
    }
}