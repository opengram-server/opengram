namespace MyTelegram.Domain.Events.Temp;

public class SetChatWallPaperStartedEvent(
    RequestInfo requestInfo,
    bool forBoth,
    bool revert,
    Peer peer,
    long? wallPaperId,
    WallPaperSettings? wallPaperSettings) : RequestAggregateEvent2<TempAggregate, TempId>(requestInfo)
{
    public bool ForBoth { get; } = forBoth;
    public bool Revert { get; } = revert;
    public Peer Peer { get; } = peer;
    public long? WallPaperId { get; } = wallPaperId;
    public WallPaperSettings? WallPaperSettings { get; } = wallPaperSettings;
}