using MyTelegram.ReadModel.Interfaces;

namespace MyTelegram.Queries;

public class GetWallpaperByIdQuery(long wallpaperId) : IQuery<IWallpaperReadModel?>
{
    public long WallpaperId { get; } = wallpaperId;
}
