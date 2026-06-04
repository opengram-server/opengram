namespace MyTelegram.Domain.Commands.User;

public class UpdateUserEmojiStatusCommand(
    UserId aggregateId,
    RequestInfo requestInfo,
    long? emojiStatusDocumentId,
    int? emojiStatusValidUntil,
    long? emojiStatusCollectibleId = null)
    : RequestCommand2<UserAggregate, UserId, IExecutionResult>(aggregateId, requestInfo)
{
    public long? EmojiStatusDocumentId { get; } = emojiStatusDocumentId;
    public int? EmojiStatusValidUntil { get; } = emojiStatusValidUntil;
    public long? EmojiStatusCollectibleId { get; } = emojiStatusCollectibleId;
}
