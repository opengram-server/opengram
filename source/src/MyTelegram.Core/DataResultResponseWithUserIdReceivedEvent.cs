using System.Buffers;

namespace MyTelegram.Core;

public record DataResultResponseWithUserIdReceivedEvent(
    long ReqMsgId,
    byte[] Data,
    long UserId,
    long AuthKeyId,
    long PermAuthKeyId) : ISessionMessage
{
    public IMemoryOwner<byte>? MemoryOwner { get; set; }
}