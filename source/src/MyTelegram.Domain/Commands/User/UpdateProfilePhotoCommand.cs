namespace MyTelegram.Domain.Commands.User;

public class UpdateProfilePhotoCommand(
    UserId aggregateId,
    RequestInfo requestInfo,
    long? photoId,
    bool fallback)
    : RequestCommand2<UserAggregate, UserId, IExecutionResult>(aggregateId, requestInfo)
{
    public long? PhotoId { get; } = photoId;
    public bool Fallback { get; } = fallback;
}