namespace MyTelegram.QueryHandlers.MongoDB.Messaging;

// ReSharper disable once UnusedMember.Global
public class GetMessagesByIdListQueryHandler(IQueryOnlyReadModelStore<MessageReadModel> store) : IQueryHandler<GetMessagesByIdListQuery,
    IReadOnlyCollection<IMessageReadModel>>
{
    public async Task<IReadOnlyCollection<IMessageReadModel>> ExecuteQueryAsync(
        GetMessagesByIdListQuery query,
        CancellationToken cancellationToken)
    {
        return await store
                .FindAsync(p => query.MessageIdList.Contains(p.Id), cancellationToken: cancellationToken);
    }
}
