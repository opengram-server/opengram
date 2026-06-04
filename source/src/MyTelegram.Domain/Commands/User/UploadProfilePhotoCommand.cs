namespace MyTelegram.Domain.Commands.User;

public class UploadProfilePhotoCommand(
    UserId aggregateId,
    RequestInfo requestInfo,
    long photoId,
    bool fallback,
    IVideoSize? videoEmojiMarkup)
    : RequestCommand2<UserAggregate, UserId, IExecutionResult>(aggregateId, requestInfo)
{
    public long PhotoId { get; } = photoId;
    public bool Fallback { get; } = fallback;
    public IVideoSize? VideoEmojiMarkup { get; } = videoEmojiMarkup;
}