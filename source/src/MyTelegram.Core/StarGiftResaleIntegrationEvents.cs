using System.Buffers;

namespace MyTelegram.Core;

public record StarGiftListedForResaleIntegrationEvent(
    string AggregateId,
    long GiftId,
    long ResaleStars
) : IMayHaveMemoryOwner
{
    public IMemoryOwner<byte>? MemoryOwner { get; set; }
}

public record StarGiftRemovedFromResaleIntegrationEvent(
    string AggregateId,
    long GiftId
) : IMayHaveMemoryOwner
{
    public IMemoryOwner<byte>? MemoryOwner { get; set; }
}