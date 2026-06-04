namespace MyTelegram.QueryHandlers.MongoDB.Messaging;

// ReSharper disable once UnusedMember.Global
public class GetMessagesByMessageIdListQueryHandler(IQueryOnlyReadModelStore<MessageReadModel> store) : IQueryHandler<GetMessagesByMessageIdListQuery,
    IReadOnlyCollection<IMessageReadModel>>
{
    public async Task<IReadOnlyCollection<IMessageReadModel>> ExecuteQueryAsync(
        GetMessagesByMessageIdListQuery query,
        CancellationToken cancellationToken)
    {
        return await store
            .FindAsync(p => query.MessageIdList.Contains(p.MessageId), cancellationToken: cancellationToken);
    }
}
