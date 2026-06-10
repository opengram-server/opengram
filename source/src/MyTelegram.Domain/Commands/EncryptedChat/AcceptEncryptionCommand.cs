using MyTelegram.Domain.Aggregates.EncryptedChat;

namespace MyTelegram.Domain.Commands.EncryptedChat;

public class AcceptEncryptionCommand(
    EncryptedChatId aggregateId,
    RequestInfo requestInfo,
    byte[] gB,
    long keyFingerprint,
    long participantPermAuthKeyId)
    : RequestCommand2<EncryptedChatAggregate, EncryptedChatId, IExecutionResult>(aggregateId, requestInfo)
{
    public byte[] GB { get; } = gB;
    public long KeyFingerprint { get; } = keyFingerprint;
    public long ParticipantPermAuthKeyId { get; } = participantPermAuthKeyId;

    protected override IEnumerable<byte[]> GetSourceIdComponents()
    {
        yield return BitConverter.GetBytes(KeyFingerprint);
    }
}
