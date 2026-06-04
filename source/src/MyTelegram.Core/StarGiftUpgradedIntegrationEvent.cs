using System.Buffers;

namespace MyTelegram.Core;

public record StarGiftUpgradedIntegrationEvent(
    string AggregateId,
    long FromUserId,      // Даритель
    long ToUserId,        // Улучшатель
    long GiftId,
    string UniqueSlug,    // Изменено с string? на string
    int UniqueNum,
    ReadOnlyMemory<byte> Attributes,   // Сериализованные атрибуты
    int UpgradeDate,
    int UpgradeMsgId
) : IMayHaveMemoryOwner
{
    public IMemoryOwner<byte>? MemoryOwner { get; set; }
};