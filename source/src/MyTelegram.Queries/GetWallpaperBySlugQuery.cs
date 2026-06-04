using MyTelegram.ReadModel.Interfaces;

namespace MyTelegram.Queries;

public class GetWallpaperBySlugQuery(string slug) : IQuery<IWallpaperReadModel?>
{
    public string Slug { get; } = slug;
}
