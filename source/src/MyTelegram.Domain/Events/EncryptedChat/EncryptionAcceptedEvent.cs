using MyTelegram.Domain.Aggregates.EncryptedChat;

namespace MyTelegram.Domain.Events.EncryptedChat;

public class EncryptionAcceptedEvent(
    RequestInfo requestInfo,
    int chatId,
    long accessHash,
    long adminId,
    long participantId,
    byte[] gB,
    long keyFingerprint,
    long participantPermAuthKeyId,
    byte[] gA)
    : RequestAggregateEvent2<EncryptedChatAggregate, EncryptedChatId>(requestInfo)
{
    public int ChatId { get; } = chatId;
    public long AccessHash { get; } = accessHash;
    public long AdminId { get; } = adminId;
    public long ParticipantId { get; } = participantId;
    public byte[] GB { get; } = gB;
    public long KeyFingerprint { get; } = keyFingerprint;
    public long ParticipantPermAuthKeyId { get; } = participantPermAuthKeyId;
    public byte[] GA { get; } = gA;
}
