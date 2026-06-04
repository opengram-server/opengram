namespace MyTelegram.Domain.Events.User;

public class UserEmojiStatusUpdatedEvent(
    RequestInfo requestInfo, 
    long userId, 
    long? emojiStatusDocumentId, 
    int? emojiStatusValidUntil,
    long? emojiStatusCollectibleId = null)
    : RequestAggregateEvent2<UserAggregate, UserId>(requestInfo)
{
    public long UserId { get; } = userId;
    public long? EmojiStatusDocumentId { get; } = emojiStatusDocumentId;
    public int? EmojiStatusValidUntil { get; } = emojiStatusValidUntil;
    public long? EmojiStatusCollectibleId { get; } = emojiStatusCollectibleId;
}
