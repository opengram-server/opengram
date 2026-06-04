using MyTelegram.ReadModel.Impl;

namespace MyTelegram.QueryHandlers.MongoDB;

public class GetChatThemeQueryHandler : IQueryHandler<GetChatThemeQuery, IChatThemeReadModel?>
{
    private readonly IMongoDatabase _database;

    public GetChatThemeQueryHandler(IMongoDatabase database)
    {
        _database = database;
    }

    public async Task<IChatThemeReadModel?> ExecuteQueryAsync(
        GetChatThemeQuery query, 
        CancellationToken cancellationToken)
    {
        var collection = _database.GetCollection<ChatThemeReadModel>("chat_themes");
        
        var filter = Builders<ChatThemeReadModel>.Filter.And(
            Builders<ChatThemeReadModel>.Filter.Eq(x => x.UserId, query.UserId),
            Builders<ChatThemeReadModel>.Filter.Eq(x => x.ChatId, query.ToPeerId)
        );
        
        return await collection.Find(filter).FirstOrDefaultAsync(cancellationToken);
    }
}
