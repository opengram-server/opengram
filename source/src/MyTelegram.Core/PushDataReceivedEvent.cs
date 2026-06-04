using System.Buffers;

namespace MyTelegram.Core;

public record PushDataReceivedEvent(
    uint ObjectId,
    long UserId,
    long ReqMsgId,
    int SeqNumber,
    long AuthKeyId,
    long PermAuthKeyId,
    ReadOnlyMemory<byte> Data,
    int Layer) : IMayHaveMemoryOwner
{
    public IMemoryOwner<byte>? MemoryOwner { get; set; }
}