using MongoDB.Driver;

namespace MyTelegram.QueryHandlers.MongoDB.User;

public class GetAllUserIdsQueryHandler : IQueryHandler<GetAllUserIdsQuery, IReadOnlyCollection<long>>
{
    private readonly IMongoDatabase _database;

    public GetAllUserIdsQueryHandler(IMongoDatabase database)
    {
        _database = database;
    }

    public async Task<IReadOnlyCollection<long>> ExecuteQueryAsync(
        GetAllUserIdsQuery query,
        CancellationToken cancellationToken)
    {
        var collection = _database.GetCollection<MyTelegram.ReadModel.MongoDB.UserReadModel>("ReadModel-UserReadModel");
        
        var projection = Builders<MyTelegram.ReadModel.MongoDB.UserReadModel>.Projection
            .Include(x => x.UserId)
            .Exclude("_id");

        var userIds = await collection
            .Find(Builders<MyTelegram.ReadModel.MongoDB.UserReadModel>.Filter.Empty)
            .Project(x => x.UserId)
            .ToListAsync(cancellationToken);

        return userIds;
    }
}
