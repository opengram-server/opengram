namespace MyTelegram.Domain.Events.User;

public class UserProfilePhotoUploadedEvent(
    RequestInfo requestInfo,
    long photoId,
    bool fallback,
    IVideoSize? videoEmojiMarkup,
    int date
    )
    : RequestAggregateEvent2<UserAggregate, UserId>(requestInfo)
{
    public long PhotoId { get; } = photoId;

    public bool Fallback { get; } = fallback;

    public IVideoSize? VideoEmojiMarkup { get; } = videoEmojiMarkup;
    public int Date { get; } = date;
}