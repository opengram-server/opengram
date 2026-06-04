using MongoDB.Driver;

namespace MyTelegram.EventFlow.MongoDB;

public class MongoDbContext(IMongoDatabase mongoDatabase) : IMongoDbContext
{
    public IMongoDatabase GetDatabase() => mongoDatabase;
}