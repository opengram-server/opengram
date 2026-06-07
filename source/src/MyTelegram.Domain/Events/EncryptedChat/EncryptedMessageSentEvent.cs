using MyTelegram.Domain.Aggregates.EncryptedChat;

namespace MyTelegram.Domain.Events.EncryptedChat;

public class EncryptedMessageSentEvent(
    RequestInfo requestInfo,
    int chatId,
    long accessHash,
    long adminId,
    long participantId,
    long adminPermAuthKeyId,
    long participantPermAuthKeyId,
    long randomId,
    byte[] data,
    byte[]? fileData,
    SendMessageType messageType,
    int date)
    : RequestAggregateEvent2<EncryptedChatAggregate, EncryptedChatId>(requestInfo)
{
    public int ChatId { get; } = chatId;
    public long AccessHash { get; } = accessHash;
    public long AdminId { get; } = adminId;
    public long ParticipantId { get; } = participantId;
    public long AdminPermAuthKeyId { get; } = adminPermAuthKeyId;
    public long ParticipantPermAuthKeyId { get; } = participantPermAuthKeyId;
    public long RandomId { get; } = randomId;
    public byte[] Data { get; } = data;
    public byte[]? FileData { get; } = fileData;
    public SendMessageType MessageType { get; } = messageType;
    public int Date { get; } = date;
}
