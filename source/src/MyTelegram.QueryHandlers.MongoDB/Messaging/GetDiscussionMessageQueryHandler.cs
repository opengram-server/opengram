namespace MyTelegram.QueryHandlers.MongoDB.Messaging;

public class GetDiscussionMessageQueryHandler(IQueryOnlyReadModelStore<MessageReadModel> store) : IQueryHandler<GetDiscussionMessageQuery, IMessageReadModel?>
{
    public async Task<IMessageReadModel?> ExecuteQueryAsync(GetDiscussionMessageQuery query,
        CancellationToken cancellationToken)
    {
        if (query.GetDiscussionMessageFromPostChannel)
        {
            return await store.FirstOrDefaultAsync(p =>
                    p.SavedFromPeerId == query.SavedFromPeerId && p.SavedFromMsgId == query.SavedFromMessageId,
                cancellationToken: cancellationToken);
        }

        return await store.FirstOrDefaultAsync(p =>
                p.OwnerPeerId == query.SavedFromPeerId && p.MessageId == query.SavedFromMessageId,
            cancellationToken: cancellationToken);
    }
}