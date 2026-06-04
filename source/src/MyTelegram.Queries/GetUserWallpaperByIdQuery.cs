namespace MyTelegram.Queries;

public class GetUserWallpaperByIdQuery(long userId, long wallpaperId) : IQuery<IUserWallPaperReadModel?>
{
    public long UserId { get; } = userId;
    public long WallpaperId { get; } = wallpaperId;
}
