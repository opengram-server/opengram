using MyTelegram.Domain.Aggregates.EncryptedChat;

namespace MyTelegram.Domain.Events.EncryptedChat;

public class EncryptionRequestedEvent(
    RequestInfo requestInfo,
    int chatId,
    long accessHash,
    long adminId,
    long participantId,
    long adminPermAuthKeyId,
    byte[] gA,
    int date)
    : RequestAggregateEvent2<EncryptedChatAggregate, EncryptedChatId>(requestInfo)
{
    public int ChatId { get; } = chatId;
    public long AccessHash { get; } = accessHash;
    public long AdminId { get; } = adminId;
    public long ParticipantId { get; } = participantId;
    public long AdminPermAuthKeyId { get; } = adminPermAuthKeyId;
    public byte[] GA { get; } = gA;
    public int Date { get; } = date;
}
