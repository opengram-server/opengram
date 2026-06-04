using System.Buffers;
using MyTelegram.Abstractions;

namespace MyTelegram.Services.Serializers;

public class StarGiftListedForResaleIntegrationEventSerializer : IEventDataSerializer<StarGiftListedForResaleIntegrationEvent>, ITransientDependency
{
    public object Deserialize(Type type, ReadOnlyMemory<byte> buffer)
    {
        return Deserialize(buffer);
    }
    
    public void Serialize(IBufferWriter<byte> writer, StarGiftListedForResaleIntegrationEvent data)
    {
        writer.WriteString(data.AggregateId);
        writer.Write(data.GiftId);
        writer.Write(data.ResaleStars);
    }

    public StarGiftListedForResaleIntegrationEvent Deserialize(ReadOnlyMemory<byte> buffer)
    {
        var aggregateId = buffer.ReadString2() ?? string.Empty;
        var giftId = buffer.ReadInt64();
        var resaleStars = buffer.ReadInt64();

        return new StarGiftListedForResaleIntegrationEvent(aggregateId, giftId, resaleStars);
    }
}

public class StarGiftRemovedFromResaleIntegrationEventSerializer : IEventDataSerializer<StarGiftRemovedFromResaleIntegrationEvent>, ITransientDependency
{
    public object Deserialize(Type type, ReadOnlyMemory<byte> buffer)
    {
        return Deserialize(buffer);
    }
    
    public void Serialize(IBufferWriter<byte> writer, StarGiftRemovedFromResaleIntegrationEvent data)
    {
        writer.WriteString(data.AggregateId);
        writer.Write(data.GiftId);
    }

    public StarGiftRemovedFromResaleIntegrationEvent Deserialize(ReadOnlyMemory<byte> buffer)
    {
        var aggregateId = buffer.ReadString2() ?? string.Empty;
        var giftId = buffer.ReadInt64();

        return new StarGiftRemovedFromResaleIntegrationEvent(aggregateId, giftId);
    }
}