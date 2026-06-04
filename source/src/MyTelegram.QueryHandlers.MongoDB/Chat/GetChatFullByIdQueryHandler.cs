using MyTelegram.Queries;
using MyTelegram.ReadModel.Impl;
using MyTelegram.ReadModel.Interfaces;

namespace MyTelegram.QueryHandlers.MongoDB.Chat;

public class GetChatFullByIdQueryHandler(IQueryOnlyReadModelStore<ChatFullReadModel> store)
    : IQueryHandler<GetChatFullByIdQuery, IChatFullReadModel?>
{
    public async Task<IChatFullReadModel?> ExecuteQueryAsync(GetChatFullByIdQuery query, CancellationToken cancellationToken)
    {
        return await store.FirstOrDefaultAsync(p => p.ChatId == query.ChatId, cancellationToken);
    }
}
