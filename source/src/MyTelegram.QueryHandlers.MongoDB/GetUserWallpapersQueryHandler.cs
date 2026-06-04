using MyTelegram.ReadModel.Impl;

namespace MyTelegram.QueryHandlers.MongoDB;

public class GetUserWallpapersQueryHandler : 
    IQueryHandler<GetUserWallpapersQuery, IReadOnlyList<IWallpaperReadModel>>
{
    private readonly IMongoDatabase _database;

    public GetUserWallpapersQueryHandler(IMongoDatabase database)
    {
        _database = database;
    }

    public async Task<IReadOnlyList<IWallpaperReadModel>> ExecuteQueryAsync(
        GetUserWallpapersQuery query, 
        CancellationToken cancellationToken)
    {
        var collection = _database.GetCollection<UserWallpaperReadModel>("user_wallpapers");
        
        var filter = Builders<UserWallpaperReadModel>.Filter.And(
            Builders<UserWallpaperReadModel>.Filter.Eq(x => x.UserId, query.UserId),
            Builders<UserWallpaperReadModel>.Filter.Eq(x => x.IsSaved, true)
        );
        
        var userWallpapers = await collection.Find(filter).ToListAsync(cancellationToken);
        
        // Get wallpaper details
        var wallpaperCollection = _database.GetCollection<WallpaperReadModel>("wallpapers");
        var wallpaperIds = userWallpapers.Select(uw => uw.WallPaperId).ToList();
        
        var wallpaperFilter = Builders<WallpaperReadModel>.Filter.In(x => x.WallpaperId, wallpaperIds);
        var wallpapers = await wallpaperCollection.Find(wallpaperFilter).ToListAsync(cancellationToken);
        
        return wallpapers.Cast<IWallpaperReadModel>().ToList();
    }
}
