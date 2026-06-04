namespace MyTelegram.Abstractions;
public record UnencryptedMessageResponse(long AuthKeyId,
    ReadOnlyMemory<byte> Data,
    string ConnectionId,
    long ReqMsgId) : IMayHaveMemoryOwner
{
    public IMemoryOwner<byte>? MemoryOwner { get; set; }
}