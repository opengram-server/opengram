using MyTelegram.ReadModel.Interfaces;

namespace MyTelegram.Queries;

public class GetAllWallpapersQuery : IQuery<IReadOnlyList<IWallpaperReadModel>>
{
    // Returns all wallpapers (default wallpapers for users who haven't saved any)
}
