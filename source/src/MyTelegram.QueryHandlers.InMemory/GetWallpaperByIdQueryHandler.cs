namespace MyTelegram.QueryHandlers.InMemory;

public class GetWallpaperByIdQueryHandler(IReadModelStore<WallpaperReadModel> store)
    : IQueryHandler<GetWallpaperByIdQuery, IWallpaperReadModel?>
{
    public async Task<IWallpaperReadModel?> ExecuteQueryAsync(GetWallpaperByIdQuery query, CancellationToken cancellationToken)
    {
        var id = WallpaperId.With(query.WallpaperId.ToString()).Value;
        return await store.GetAsync(id, cancellationToken);
    }
}
