namespace MyTelegram.QueryHandlers.InMemory;

public class GetChatThemeQueryHandler(IReadModelStore<ChatThemeReadModel> store)
    : IQueryHandler<GetChatThemeQuery, IChatThemeReadModel?>
{
    public async Task<IChatThemeReadModel?> ExecuteQueryAsync(GetChatThemeQuery query, CancellationToken cancellationToken)
    {
        var id = $"{query.UserId}_{query.ChatId}";
        return await store.GetAsync(id, cancellationToken);
    }
}
