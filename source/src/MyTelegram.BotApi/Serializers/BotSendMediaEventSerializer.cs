using System.Buffers;
using MyTelegram.Abstractions;
using MyTelegram.Domain.Shared.Events;
using MessagePack;

namespace MyTelegram.BotApi.Serializers;

public class BotSendMediaEventSerializer : IEventDataSerializer<BotSendMediaEvent>
{
    private static readonly MessagePackSerializerOptions Options = MessagePackSerializerOptions.Standard
        .WithResolver(MessagePack.Resolvers.ContractlessStandardResolver.Instance);

    public object Deserialize(Type type, ReadOnlyMemory<byte> buffer)
    {
        return MessagePackSerializer.Deserialize<BotSendMediaEvent>(buffer, Options)!;
    }

    public BotSendMediaEvent Deserialize(ReadOnlyMemory<byte> buffer)
    {
        return MessagePackSerializer.Deserialize<BotSendMediaEvent>(buffer, Options);
    }

    public void Serialize(IBufferWriter<byte> writer, BotSendMediaEvent value)
    {
        MessagePackSerializer.Serialize(writer, value, Options);
    }
}
