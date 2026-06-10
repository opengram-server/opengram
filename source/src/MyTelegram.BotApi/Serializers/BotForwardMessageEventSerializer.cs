using System.Buffers;
using MyTelegram.Abstractions;
using MyTelegram.Domain.Shared.Events;
using MessagePack;

namespace MyTelegram.BotApi.Serializers;

public class BotForwardMessageEventSerializer : IEventDataSerializer<BotForwardMessageEvent>
{
    private static readonly MessagePackSerializerOptions Options = MessagePackSerializerOptions.Standard
        .WithResolver(MessagePack.Resolvers.ContractlessStandardResolver.Instance);

    public object Deserialize(Type type, ReadOnlyMemory<byte> buffer)
    {
        return MessagePackSerializer.Deserialize<BotForwardMessageEvent>(buffer, Options)!;
    }

    public BotForwardMessageEvent Deserialize(ReadOnlyMemory<byte> buffer)
    {
        return MessagePackSerializer.Deserialize<BotForwardMessageEvent>(buffer, Options);
    }

    public void Serialize(IBufferWriter<byte> writer, BotForwardMessageEvent value)
    {
        MessagePackSerializer.Serialize(writer, value, Options);
    }
}
