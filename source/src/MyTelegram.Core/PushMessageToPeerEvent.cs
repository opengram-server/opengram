using System.Buffers;

namespace MyTelegram.Core;

public record PushMessageToPeerEvent(
    int PeerType,
    long PeerId,
    ReadOnlyMemory<byte> Data,
    long ExcludeAuthKeyId,
    long ExcludeUid,
    long OnlySendToThisAuthKeyId,
    int Pts,
    long GlobalSeqNo) : ISessionMessage
{
    public IMemoryOwner<byte>? MemoryOwner { get; set; }
}