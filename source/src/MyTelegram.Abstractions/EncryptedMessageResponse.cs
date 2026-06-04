namespace MyTelegram.Abstractions;

public record EncryptedMessageResponse(long AuthKeyId,
    ReadOnlyMemory<byte> Data,
    string ConnectionId,
    long SeqNumber
) : IMayHaveMemoryOwner
{
    public IMemoryOwner<byte>? MemoryOwner { get; set; }
}
