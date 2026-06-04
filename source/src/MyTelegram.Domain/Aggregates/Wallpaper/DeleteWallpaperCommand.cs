using MyTelegram.Domain.Commands;

namespace MyTelegram.Domain.Aggregates.Wallpaper;

public class DeleteWallpaperCommand(
    WallpaperId aggregateId,
    RequestInfo requestInfo,
    long userId)
    : RequestCommand2<WallpaperAggregate, WallpaperId, IExecutionResult>(aggregateId, requestInfo)
{
    public long UserId { get; } = userId;

    protected override IEnumerable<byte[]> GetSourceIdComponents()
    {
        yield return BitConverter.GetBytes(RequestInfo.ReqMsgId);
        yield return RequestInfo.RequestId.ToByteArray();
    }
}
