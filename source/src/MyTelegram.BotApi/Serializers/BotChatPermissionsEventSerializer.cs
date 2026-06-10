using System.Buffers;
using MyTelegram.Abstractions;
using MyTelegram.Domain.Shared.Events;
using MessagePack;

namespace MyTelegram.BotApi.Serializers;

public class BotChatPermissionsEventSerializer : IEventDataSerializer<BotChatPermissionsEvent>
{
    private static readonly MessagePackSerializerOptions Options = MessagePackSerializerOptions.Standard
        .WithResolver(MessagePack.Resolvers.ContractlessStandardResolver.Instance);

    public object Deserialize(Type type, ReadOnlyMemory<byte> buffer)
    {
        return MessagePackSerializer.Deserialize<BotChatPermissionsEvent>(buffer, Options)!;
    }

    public BotChatPermissionsEvent Deserialize(ReadOnlyMemory<byte> buffer)
    {
        return MessagePackSerializer.Deserialize<BotChatPermissionsEvent>(buffer, Options);
    }

    public void Serialize(IBufferWriter<byte> writer, BotChatPermissionsEvent value)
    {
        MessagePackSerializer.Serialize(writer, value, Options);
    }
}
