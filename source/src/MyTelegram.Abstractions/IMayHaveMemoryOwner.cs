namespace MyTelegram.Abstractions;

public interface IMayHaveMemoryOwner
{
    IMemoryOwner<byte>? MemoryOwner { get; set; }
}