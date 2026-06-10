using System.Buffers;
using MyTelegram.Abstractions;
using MyTelegram.Domain.Shared.Events;
using MessagePack;

namespace MyTelegram.BotApi.Serializers;

public class BotEditMessageEventSerializer : IEventDataSerializer<BotEditMessageEvent>
{
    private static readonly MessagePackSerializerOptions Options = MessagePackSerializerOptions.Standard
        .WithResolver(MessagePack.Resolvers.ContractlessStandardResolver.Instance);

    public object Deserialize(Type type, ReadOnlyMemory<byte> buffer)
    {
        return MessagePackSerializer.Deserialize<BotEditMessageEvent>(buffer, Options)!;
    }

    public BotEditMessageEvent Deserialize(ReadOnlyMemory<byte> buffer)
    {
        return MessagePackSerializer.Deserialize<BotEditMessageEvent>(buffer, Options);
    }

    public void Serialize(IBufferWriter<byte> writer, BotEditMessageEvent value)
    {
        MessagePackSerializer.Serialize(writer, value, Options);
    }
}
