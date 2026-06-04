using System.Buffers;
using MyTelegram.Schema;

namespace MyTelegram.Core;

public record DataResultResponseReceivedEvent(
    long ReqMsgId,
    ReadOnlyMemory<byte> Data
) : ISessionMessage
{
   public IObject? DataObject { get; set; }
   public IMemoryOwner<byte>? MemoryOwner { get; set; }
}