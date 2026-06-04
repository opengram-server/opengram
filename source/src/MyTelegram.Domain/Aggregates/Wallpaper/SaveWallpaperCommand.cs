using MyTelegram.Domain.Commands;

namespace MyTelegram.Domain.Aggregates.Wallpaper;

public class SaveWallpaperCommand(
    WallpaperId aggregateId,
    RequestInfo requestInfo,
    long userId,
    bool unsave,
    WallPaperSettings? settings)
    : RequestCommand2<WallpaperAggregate, WallpaperId, IExecutionResult>(aggregateId, requestInfo)
{
    public long UserId { get; } = userId;
    public bool Unsave { get; } = unsave;
    public WallPaperSettings? Settings { get; } = settings;

    protected override IEnumerable<byte[]> GetSourceIdComponents()
    {
        yield return BitConverter.GetBytes(RequestInfo.ReqMsgId);
        yield return RequestInfo.RequestId.ToByteArray();
    }
}
