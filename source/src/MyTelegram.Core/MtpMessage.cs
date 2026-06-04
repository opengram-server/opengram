namespace MyTelegram.Core;

public record MtpMessage(
    long ServerSalt,
    long SessionId,
    long MessageId,
    int SeqNumber,
    int MessageDataLength,
    ReadOnlyMemory<byte> MessageData);
