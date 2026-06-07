using MyTelegram.Domain.Aggregates.EncryptedChat;

namespace MyTelegram.Domain.Events.EncryptedChat;

public class EncryptionDiscardedEvent(
    RequestInfo requestInfo,
    int chatId,
    long adminId,
    long participantId,
    bool deleteHistory)
    : RequestAggregateEvent2<EncryptedChatAggregate, EncryptedChatId>(requestInfo)
{
    public int ChatId { get; } = chatId;
    public long AdminId { get; } = adminId;
    public long ParticipantId { get; } = participantId;
    public bool DeleteHistory { get; } = deleteHistory;
}
