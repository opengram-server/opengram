namespace MyTelegram.ReadModel.Interfaces;

public interface IWallpaperReadModel : IReadModel
{
    string Id { get; }
    long WallpaperId { get; }
    //bool Creator { get; }
    long UserId { get; }
    bool Default { get; }
    bool Pattern { get; }
    bool Dark { get; }
    long AccessHash { get; }
    string Slug { get; }
    long? DocumentId { get; }
    WallPaperSettings? Settings { get; }
    bool IsDeleted { get; }
}