using MyTelegram.ReadModel.Impl;

namespace MyTelegram.QueryHandlers.MongoDB;

public class GetWallpaperBySlugQueryHandler : 
    IQueryHandler<GetWallpaperBySlugQuery, IWallpaperReadModel?>
{
    private readonly IMongoDatabase _database;

    public GetWallpaperBySlugQueryHandler(IMongoDatabase database)
    {
        _database = database;
    }

    public async Task<IWallpaperReadModel?> ExecuteQueryAsync(
        GetWallpaperBySlugQuery query, 
        CancellationToken cancellationToken)
    {
        var collection = _database.GetCollection<WallpaperReadModel>("wallpapers");
        
        var filter = Builders<WallpaperReadModel>.Filter.And(
            Builders<WallpaperReadModel>.Filter.Eq(x => x.Slug, query.Slug),
            Builders<WallpaperReadModel>.Filter.Eq(x => x.IsDeleted, false)
        );
        
        return await collection.Find(filter).FirstOrDefaultAsync(cancellationToken);
    }
}
