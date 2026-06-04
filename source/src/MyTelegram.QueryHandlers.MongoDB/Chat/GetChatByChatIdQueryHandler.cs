using EventFlow.MongoDB.ReadStores;
using EventFlow.Queries;
using MongoDB.Driver;
using MyTelegram.Queries;
using MyTelegram.ReadModel.Impl;
using MyTelegram.ReadModel.Interfaces;

namespace MyTelegram.QueryHandlers.MongoDB.Chat;

public class GetChatByChatIdQueryHandler(IMongoDbReadModelStore<ChatReadModel> store)
    : IQueryHandler<GetChatByChatIdQuery, IChatReadModel?>
{
    public async Task<IChatReadModel?> ExecuteQueryAsync(GetChatByChatIdQuery query, CancellationToken cancellationToken)
    {
        var readModel = await store.FindAsync(p => p.ChatId == query.ChatId, cancellationToken: cancellationToken);
        return readModel.FirstOrDefault();
    }
}
