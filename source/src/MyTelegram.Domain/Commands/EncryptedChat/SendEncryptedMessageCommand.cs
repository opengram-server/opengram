using MyTelegram.Domain.Aggregates.EncryptedChat;

namespace MyTelegram.Domain.Commands.EncryptedChat;

public class SendEncryptedMessageCommand(
    EncryptedChatId aggregateId,
    RequestInfo requestInfo,
    long randomId,
    byte[] data,
    byte[]? fileData,
    SendMessageType messageType,
    int date)
    : RequestCommand2<EncryptedChatAggregate, EncryptedChatId, IExecutionResult>(aggregateId, requestInfo)
{
    public long RandomId { get; } = randomId;
    public byte[] Data { get; } = data;
    public byte[]? FileData { get; } = fileData;
    public SendMessageType MessageType { get; } = messageType;
    public int Date { get; } = date;

    protected override IEnumerable<byte[]> GetSourceIdComponents()
    {
        yield return BitConverter.GetBytes(RandomId);
    }
}
