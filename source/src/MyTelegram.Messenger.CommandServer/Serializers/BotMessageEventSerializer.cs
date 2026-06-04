using System.Buffers;
using MyTelegram.Abstractions;
using MyTelegram.Domain.Shared.Events;
using MessagePack;

namespace MyTelegram.Messenger.CommandServer.Serializers;

public class BotMessageEventSerializer : IEventDataSerializer<BotMessageEvent>, ITransientDependency
{
    private static readonly MessagePackSerializerOptions Options = MessagePackSerializerOptions.Standard
        .WithResolver(MessagePack.Resolvers.ContractlessStandardResolver.Instance);

    public object Deserialize(Type type, ReadOnlyMemory<byte> buffer)
    {
        return MessagePackSerializer.Deserialize<BotMessageEvent>(buffer, Options)!;
    }

    public BotMessageEvent Deserialize(ReadOnlyMemory<byte> buffer)
    {
        return MessagePackSerializer.Deserialize<BotMessageEvent>(buffer, Options);
    }

    public void Serialize(IBufferWriter<byte> writer, BotMessageEvent value)
    {
        MessagePackSerializer.Serialize(writer, value, Options);
    }
}
