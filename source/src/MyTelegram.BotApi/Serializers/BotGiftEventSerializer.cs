using System.Buffers;
using MyTelegram.Abstractions;
using MyTelegram.Domain.Shared.Events;
using MessagePack;

namespace MyTelegram.BotApi.Serializers;

public class BotGiftEventSerializer : IEventDataSerializer<BotGiftEvent>
{
    private static readonly MessagePackSerializerOptions Options = MessagePackSerializerOptions.Standard
        .WithResolver(MessagePack.Resolvers.ContractlessStandardResolver.Instance);

    public object Deserialize(Type type, ReadOnlyMemory<byte> buffer)
    {
        return MessagePackSerializer.Deserialize<BotGiftEvent>(buffer, Options)!;
    }

    public BotGiftEvent Deserialize(ReadOnlyMemory<byte> buffer)
    {
        return MessagePackSerializer.Deserialize<BotGiftEvent>(buffer, Options);
    }

    public void Serialize(IBufferWriter<byte> writer, BotGiftEvent value)
    {
        MessagePackSerializer.Serialize(writer, value, Options);
    }
}
