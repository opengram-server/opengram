using MyTelegram.Queries;
using MyTelegram.ReadModel.Interfaces;
using MyTelegram.ReadModel.Impl;
using MongoDB.Driver;

namespace MyTelegram.QueryHandlers.MongoDB.QuickReply;

public class GetQuickRepliesByUserIdQueryHandler(IMongoDbReadModelStore<QuickReplyReadModel> store)
    : IQueryHandler<GetQuickRepliesByUserIdQuery, IReadOnlyCollection<IQuickReplyReadModel>>
{
    public async Task<IReadOnlyCollection<IQuickReplyReadModel>> ExecuteQueryAsync(GetQuickRepliesByUserIdQuery query,
        CancellationToken cancellationToken)
    {
        var cursor = await store.FindAsync(p => p.UserId == query.UserId, cancellationToken: cancellationToken);
        return await cursor.ToListAsync(cancellationToken);
    }
}
