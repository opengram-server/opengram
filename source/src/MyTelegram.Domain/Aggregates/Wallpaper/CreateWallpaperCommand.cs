using MyTelegram.Domain.Commands;

namespace MyTelegram.Domain.Aggregates.Wallpaper;

public class CreateWallpaperCommand(
    WallpaperId aggregateId,
    RequestInfo requestInfo,
    long wallpaperId,
    long userId,
    long accessHash,
    string slug,
    long? documentId,
    bool isDefault,
    bool isPattern,
    bool isDark,
    WallPaperSettings? settings)
    : RequestCommand2<WallpaperAggregate, WallpaperId, IExecutionResult>(aggregateId, requestInfo)
{
    public long WallpaperId { get; } = wallpaperId;
    public long UserId { get; } = userId;
    public long AccessHash { get; } = accessHash;
    public string Slug { get; } = slug;
    public long? DocumentId { get; } = documentId;
    public bool IsDefault { get; } = isDefault;
    public bool IsPattern { get; } = isPattern;
    public bool IsDark { get; } = isDark;
    public WallPaperSettings? Settings { get; } = settings;

    protected override IEnumerable<byte[]> GetSourceIdComponents()
    {
        yield return BitConverter.GetBytes(WallpaperId);
        yield return BitConverter.GetBytes(UserId);
    }
}
