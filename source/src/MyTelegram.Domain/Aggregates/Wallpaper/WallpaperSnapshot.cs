namespace MyTelegram.Domain.Aggregates.Wallpaper;

public record WallpaperSnapshot(
    long WallpaperId,
    long AccessHash,
    long CreatorId,
    string Slug,
    long? DocumentId,
    bool IsPattern,
    bool IsDark,
    bool IsDefault,
    bool IsDeleted,
    bool ForChat,
    WallPaperSettings? Settings
) : ISnapshot;
