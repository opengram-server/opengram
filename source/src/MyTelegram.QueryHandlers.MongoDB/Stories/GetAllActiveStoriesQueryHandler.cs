using MongoDB.Driver;
using MyTelegram.Queries.Stories;
using MyTelegram.ReadModel.Impl;

namespace MyTelegram.QueryHandlers.MongoDB.Stories;

public class GetAllActiveStoriesQueryHandler : IQueryHandler<GetAllActiveStoriesQuery, IReadOnlyList<IStoryReadModel>>
{
    private readonly IMongoDatabase _database;

    public GetAllActiveStoriesQueryHandler(IMongoDatabase database)
    {
        _database = database;
    }

    public async Task<IReadOnlyList<IStoryReadModel>> ExecuteQueryAsync(GetAllActiveStoriesQuery query, CancellationToken cancellationToken)
    {
        var collection = _database.GetCollection<StoryReadModel>("ReadModel-StoryReadModel");
        
        var filter = Builders<StoryReadModel>.Filter.And(
            Builders<StoryReadModel>.Filter.Eq(x => x.IsDeleted, false),
            Builders<StoryReadModel>.Filter.Gt(x => x.ExpireDate, DateTimeOffset.UtcNow.ToUnixTimeSeconds())
        );

        var stories = await collection.Find(filter)
            .SortByDescending(x => x.Date)
            .ToListAsync(cancellationToken);

        return stories.Cast<IStoryReadModel>().ToList();
    }
}
