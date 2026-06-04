using System.Buffers;

namespace MyTelegram.Core;

public record LayeredAuthKeyIdMessageCreatedIntegrationEvent(
    long AuthKeyId,
    ReadOnlyMemory<byte> Data,
    int Pts,
    int? Qts,
    long GlobalSeqNo) : ISessionMessage
{
    public IMemoryOwner<byte>? MemoryOwner { get; set; }
}