using System.Buffers;
using MyTelegram.Abstractions;
using MyTelegram.Domain.Shared.Events;
using MessagePack;

namespace MyTelegram.BotApi.Serializers;

public class BotDeleteMessageEventSerializer : IEventDataSerializer<BotDeleteMessageEvent>
{
    private static readonly MessagePackSerializerOptions Options = MessagePackSerializerOptions.Standard
        .WithResolver(MessagePack.Resolvers.ContractlessStandardResolver.Instance);

    public object Deserialize(Type type, ReadOnlyMemory<byte> buffer)
    {
        return MessagePackSerializer.Deserialize<BotDeleteMessageEvent>(buffer, Options)!;
    }

    public BotDeleteMessageEvent Deserialize(ReadOnlyMemory<byte> buffer)
    {
        return MessagePackSerializer.Deserialize<BotDeleteMessageEvent>(buffer, Options);
    }

    public void Serialize(IBufferWriter<byte> writer, BotDeleteMessageEvent value)
    {
        MessagePackSerializer.Serialize(writer, value, Options);
    }
}
