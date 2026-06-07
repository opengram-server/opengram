using EventFlow.MongoDB.ReadStores;
using EventFlow.Queries;
using MongoDB.Driver;
using MyTelegram.Queries;
using MyTelegram.ReadModel.Impl;

namespace MyTelegram.QueryHandlers.MongoDB.Chat;

public class GetChatMemberListQueryHandler(IMongoDbReadModelStore<ChatReadModel> store)
    : IQueryHandler<GetChatMemberListQuery, IReadOnlyCollection<long>>
{
    public async Task<IReadOnlyCollection<long>> ExecuteQueryAsync(
        GetChatMemberListQuery query,
        CancellationToken cancellationToken)
    {
        var chats = await store.FindAsync(
            p => p.ChatId == query.ChatId,
            cancellationToken: cancellationToken);

        var chat = chats.FirstOrDefault();
        if (chat == null)
        {
            return Array.Empty<long>();
        }

        return chat.ChatMembers.Select(m => m.UserId).ToList();
    }
}
