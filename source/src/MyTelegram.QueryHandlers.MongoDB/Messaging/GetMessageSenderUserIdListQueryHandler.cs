namespace MyTelegram.QueryHandlers.MongoDB.Messaging;

public class GetMessageSenderUserIdListQueryHandler(IQueryOnlyReadModelStore<MessageReadModel> store) : IQueryHandler<GetMessageSenderUserIdListQuery, IReadOnlyCollection<long>>
{
    public async Task<IReadOnlyCollection<long>> ExecuteQueryAsync(GetMessageSenderUserIdListQuery query, CancellationToken cancellationToken)
    {
        return await store.FindAsync(p =>
            p.OwnerPeerId == query.ChannelId && query.MessageIds.Contains(p.MessageId), p => p.SenderUserId, cancellationToken: cancellationToken);
    }
}