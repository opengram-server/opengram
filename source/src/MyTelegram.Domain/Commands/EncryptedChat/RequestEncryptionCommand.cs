using MyTelegram.Domain.Aggregates.EncryptedChat;

namespace MyTelegram.Domain.Commands.EncryptedChat;

public class RequestEncryptionCommand(
    EncryptedChatId aggregateId,
    RequestInfo requestInfo,
    int chatId,
    long accessHash,
    long adminId,
    long participantId,
    long adminPermAuthKeyId,
    byte[] gA,
    int date)
    : RequestCommand2<EncryptedChatAggregate, EncryptedChatId, IExecutionResult>(aggregateId, requestInfo)
{
    public int ChatId { get; } = chatId;
    public long AccessHash { get; } = accessHash;
    public long AdminId { get; } = adminId;
    public long ParticipantId { get; } = participantId;
    public long AdminPermAuthKeyId { get; } = adminPermAuthKeyId;
    public byte[] GA { get; } = gA;
    public int Date { get; } = date;

    protected override IEnumerable<byte[]> GetSourceIdComponents()
    {
        yield return BitConverter.GetBytes(ChatId);
    }
}
