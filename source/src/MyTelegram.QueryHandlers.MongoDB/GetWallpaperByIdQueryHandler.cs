using MyTelegram.ReadModel.Impl;

namespace MyTelegram.QueryHandlers.MongoDB;

public class GetWallpaperByIdQueryHandler : 
    IQueryHandler<GetWallpaperByIdQuery, IWallpaperReadModel?>
{
    private readonly IMongoDatabase _database;

    public GetWallpaperByIdQueryHandler(IMongoDatabase database)
    {
        _database = database;
    }

    public async Task<IWallpaperReadModel?> ExecuteQueryAsync(
        GetWallpaperByIdQuery query, 
        CancellationToken cancellationToken)
    {
        var collection = _database.GetCollection<WallpaperReadModel>("wallpapers");
        var id = query.WallpaperId.ToString();
        
        var filter = Builders<WallpaperReadModel>.Filter.Eq(x => x.Id, id);
        return await collection.Find(filter).FirstOrDefaultAsync(cancellationToken);
    }
}
