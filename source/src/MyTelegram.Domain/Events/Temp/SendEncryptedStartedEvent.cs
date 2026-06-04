namespace MyTelegram.Domain.Events.Temp;

public class SendEncryptedStartedEvent(RequestInfo requestInfo,
    EncryptedChatItem encryptedChatItem,
    bool silent,
    long randomId,
    byte[] data,
    byte[]? fileBytes,
    int date,
    SendMessageType messageType) : RequestAggregateEvent2<TempAggregate, TempId>(requestInfo)
{
    public EncryptedChatItem EncryptedChatItem { get; } = encryptedChatItem;
    public bool Silent { get; } = silent;
    public long RandomId { get; } = randomId;
    public byte[] Data { get; } = data;
    public byte[]? FileBytes { get; } = fileBytes;
    public int Date { get; } = date;
    public SendMessageType MessageType { get; } = messageType;
}