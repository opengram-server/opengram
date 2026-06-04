using System.Buffers;

namespace MyTelegram.Core;

public interface ISessionMessage
{
    public IMemoryOwner<byte>? MemoryOwner { get; set; }
}