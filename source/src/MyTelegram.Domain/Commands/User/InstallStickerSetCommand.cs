namespace MyTelegram.Domain.Commands.User;

public class InstallStickerSetCommand(
    UserId aggregateId,
    RequestInfo requestInfo,
    long stickerSetId,
    bool archived,
    StickerSetType stickerSetType = StickerSetType.Regular)
    : RequestCommand2<UserAggregate, UserId, IExecutionResult>(aggregateId, requestInfo)
{
    public long StickerSetId { get; } = stickerSetId;
    public bool Archived { get; } = archived;
    public StickerSetType StickerSetType { get; } = stickerSetType;
}
