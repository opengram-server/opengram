using MyTelegram.ReadModel.Interfaces;

namespace MyTelegram.Queries;

public class GetUserWallpapersQuery(long userId) : IQuery<IReadOnlyList<IWallpaperReadModel>>
{
    public long UserId { get; } = userId;
}
