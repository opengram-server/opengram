using MongoDB.Driver;
using MyTelegram.Queries.User;
using MyTelegram.ReadModel.Impl;

namespace MyTelegram.QueryHandlers.MongoDB.User;

public class GetUserByCollectibleUsernameQueryHandler : IQueryHandler<GetUserByCollectibleUsernameQuery, IUserReadModel?>
{
    private readonly IMongoDatabase _database;

    public GetUserByCollectibleUsernameQueryHandler(IMongoDatabase database)
    {
        _database = database;
    }

    public async Task<IUserReadModel?> ExecuteQueryAsync(GetUserByCollectibleUsernameQuery query, CancellationToken cancellationToken)
    {
        var collection = _database.GetCollection<UserReadModel>("eventflow-userreadmodel");
        
        // Search in Usernames array field for collectible usernames
        var filter = Builders<UserReadModel>.Filter.AnyEq(x => x.Usernames, query.Username);
        
        var user = await collection.Find(filter).FirstOrDefaultAsync(cancellationToken);
        
        return user;
    }
}
