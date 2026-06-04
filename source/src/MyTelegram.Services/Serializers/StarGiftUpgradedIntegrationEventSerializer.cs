using System.Buffers;
using MyTelegram.Abstractions;

namespace MyTelegram.Services.Serializers;

public class StarGiftUpgradedIntegrationEventEventDataSerializer : IEventDataSerializer<StarGiftUpgradedIntegrationEvent>, ITransientDependency
{
    public object Deserialize(Type type, ReadOnlyMemory<byte> buffer)
    {
        return Deserialize(buffer);
    }
    
    public void Serialize(IBufferWriter<byte> writer, StarGiftUpgradedIntegrationEvent data)
    {
        writer.WriteString(data.AggregateId);
        writer.Write(data.FromUserId);
        writer.Write(data.ToUserId);
        writer.Write(data.GiftId);
        writer.WriteString(data.UniqueSlug);
        writer.Write(data.UniqueNum);
        writer.Write(data.Attributes);
        writer.Write(data.UpgradeDate);
        writer.Write(data.UpgradeMsgId);
    }

    public StarGiftUpgradedIntegrationEvent Deserialize(ReadOnlyMemory<byte> buffer)
    {
        var aggregateId = buffer.ReadString2() ?? string.Empty;
        var fromUserId = buffer.ReadInt64();
        var toUserId = buffer.ReadInt64();
        var giftId = buffer.ReadInt64();
        var uniqueSlug = buffer.ReadString2() ?? string.Empty;
        var uniqueNum = buffer.ReadInt32();
        var attributes = buffer.ReadMemory();
        var upgradeDate = buffer.ReadInt32();
        var upgradeMsgId = buffer.ReadInt32();

        var rentLength = attributes.Length;
        var owner = MemoryPool<byte>.Shared.Rent(rentLength);
        var memory = owner.Memory.Slice(0, rentLength);
        var attributesMemory = memory.Slice(0, attributes.Length);
        attributes.CopyTo(attributesMemory);
        attributes = attributesMemory;

        return new StarGiftUpgradedIntegrationEvent(
            aggregateId,
            fromUserId,
            toUserId,
            giftId,
            uniqueSlug,
            uniqueNum,
            attributes,
            upgradeDate,
            upgradeMsgId
        )
        {
            MemoryOwner = owner
        };
    }
}