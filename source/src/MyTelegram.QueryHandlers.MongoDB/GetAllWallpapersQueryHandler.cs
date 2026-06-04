using Microsoft.Extensions.Logging;
using MyTelegram.ReadModel.Impl;
using MyTelegram.ReadModel.Interfaces;

namespace MyTelegram.QueryHandlers.MongoDB;

public class GetAllWallpapersQueryHandler : 
    IQueryHandler<GetAllWallpapersQuery, IReadOnlyList<IWallpaperReadModel>>
{
    private readonly IMongoDatabase _database;
    private readonly ILogger<GetAllWallpapersQueryHandler> _logger;

    public GetAllWallpapersQueryHandler(IMongoDatabase database, ILogger<GetAllWallpapersQueryHandler> logger)
    {
        _database = database;
        _logger = logger;
    }

    public async Task<IReadOnlyList<IWallpaperReadModel>> ExecuteQueryAsync(
        GetAllWallpapersQuery query, 
        CancellationToken cancellationToken)
    {
        var collection = _database.GetCollection<WallpaperReadModel>("wallpapers");
        
        // TEMPORARY: Return ALL default wallpapers including fill wallpapers for testing
        // TODO: In production, should only return wallpapers with documents
        // Fill wallpapers (without DocumentId) are normally NOT synced via account.getWallPapers
        var filter = Builders<WallpaperReadModel>.Filter.And(
            Builders<WallpaperReadModel>.Filter.Eq(x => x.Default, true),
            Builders<WallpaperReadModel>.Filter.Eq(x => x.IsDeleted, false)
            // Removed: Filter.Ne(x => x.DocumentId, null) - allowing fill wallpapers for testing
        );
        
        var wallpapers = await collection.Find(filter).ToListAsync(cancellationToken);
        
        _logger.LogInformation("GetAllWallpapersQueryHandler: Found {Count} default wallpapers with documents", wallpapers.Count);
        
        return wallpapers.Cast<IWallpaperReadModel>().ToList();
    }
}
