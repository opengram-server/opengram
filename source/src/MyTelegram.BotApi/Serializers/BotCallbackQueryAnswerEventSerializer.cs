using System.Buffers;
using MyTelegram.Abstractions;
using MyTelegram.Domain.Shared.Events;
using MessagePack;

namespace MyTelegram.BotApi.Serializers;

public class BotCallbackQueryAnswerEventSerializer : IEventDataSerializer<BotCallbackQueryAnswerEvent>
{
    private static readonly MessagePackSerializerOptions Options = MessagePackSerializerOptions.Standard
        .WithResolver(MessagePack.Resolvers.ContractlessStandardResolver.Instance);

    public object Deserialize(Type type, ReadOnlyMemory<byte> buffer)
    {
        return MessagePackSerializer.Deserialize<BotCallbackQueryAnswerEvent>(buffer, Options)!;
    }

    public BotCallbackQueryAnswerEvent Deserialize(ReadOnlyMemory<byte> buffer)
    {
        return MessagePackSerializer.Deserialize<BotCallbackQueryAnswerEvent>(buffer, Options);
    }

    public void Serialize(IBufferWriter<byte> writer, BotCallbackQueryAnswerEvent value)
    {
        MessagePackSerializer.Serialize(writer, value, Options);
    }
}
