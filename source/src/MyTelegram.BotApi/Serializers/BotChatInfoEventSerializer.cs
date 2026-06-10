using System.Buffers;
using MyTelegram.Abstractions;
using MyTelegram.Domain.Shared.Events;
using MessagePack;

namespace MyTelegram.BotApi.Serializers;

public class BotChatInfoEventSerializer : IEventDataSerializer<BotChatInfoEvent>
{
    private static readonly MessagePackSerializerOptions Options = MessagePackSerializerOptions.Standard
        .WithResolver(MessagePack.Resolvers.ContractlessStandardResolver.Instance);

    public object Deserialize(Type type, ReadOnlyMemory<byte> buffer)
    {
        return MessagePackSerializer.Deserialize<BotChatInfoEvent>(buffer, Options)!;
    }

    public BotChatInfoEvent Deserialize(ReadOnlyMemory<byte> buffer)
    {
        return MessagePackSerializer.Deserialize<BotChatInfoEvent>(buffer, Options);
    }

    public void Serialize(IBufferWriter<byte> writer, BotChatInfoEvent value)
    {
        MessagePackSerializer.Serialize(writer, value, Options);
    }
}
