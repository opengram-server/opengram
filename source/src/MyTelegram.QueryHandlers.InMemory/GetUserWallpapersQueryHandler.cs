namespace MyTelegram.QueryHandlers.InMemory;

public class GetUserWallpapersQueryHandler(IReadModelStore<UserWallpaperReadModel> store)
    : IQueryHandler<GetUserWallpapersQuery, IReadOnlyList<IWallPaperReadModel>>
{
    public async Task<IReadOnlyList<IWallPaperReadModel>> ExecuteQueryAsync(GetUserWallpapersQuery query, CancellationToken cancellationToken)
    {
        // This is a simplified implementation
        // In production, you would query by UserId index
        var allWallpapers = await store.FindAsync(
            w => w.UserId == query.UserId && w.IsSaved,
            cancellationToken
        );

        return allWallpapers.Cast<IWallPaperReadModel>().ToList();
    }
}
