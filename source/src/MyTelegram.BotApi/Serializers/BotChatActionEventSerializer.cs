using System.Buffers;
using MyTelegram.Abstractions;
using MyTelegram.Domain.Shared.Events;
using MessagePack;

namespace MyTelegram.BotApi.Serializers;

public class BotChatActionEventSerializer : IEventDataSerializer<BotChatActionEvent>
{
    private static readonly MessagePackSerializerOptions Options = MessagePackSerializerOptions.Standard
        .WithResolver(MessagePack.Resolvers.ContractlessStandardResolver.Instance);

    public object Deserialize(Type type, ReadOnlyMemory<byte> buffer)
    {
        return MessagePackSerializer.Deserialize<BotChatActionEvent>(buffer, Options)!;
    }

    public BotChatActionEvent Deserialize(ReadOnlyMemory<byte> buffer)
    {
        return MessagePackSerializer.Deserialize<BotChatActionEvent>(buffer, Options);
    }

    public void Serialize(IBufferWriter<byte> writer, BotChatActionEvent value)
    {
        MessagePackSerializer.Serialize(writer, value, Options);
    }
}
