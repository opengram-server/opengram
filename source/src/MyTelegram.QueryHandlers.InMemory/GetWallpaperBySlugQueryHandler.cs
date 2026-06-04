namespace MyTelegram.QueryHandlers.InMemory;

public class GetWallpaperBySlugQueryHandler(IReadModelStore<WallpaperReadModel> store)
    : IQueryHandler<GetWallpaperBySlugQuery, IWallpaperReadModel?>
{
    public async Task<IWallpaperReadModel?> ExecuteQueryAsync(GetWallpaperBySlugQuery query, CancellationToken cancellationToken)
    {
        var wallpapers = await store.FindAsync(
            w => w.Slug == query.Slug && !w.IsDeleted,
            cancellationToken
        );

        return wallpapers.FirstOrDefault();
    }
}
