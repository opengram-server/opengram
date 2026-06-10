using System.Buffers;
using MyTelegram.Abstractions;
using MyTelegram.Domain.Shared.Events;
using MessagePack;

namespace MyTelegram.BotApi.Serializers;

public class BotChatPhotoEventSerializer : IEventDataSerializer<BotChatPhotoEvent>
{
    private static readonly MessagePackSerializerOptions Options = MessagePackSerializerOptions.Standard
        .WithResolver(MessagePack.Resolvers.ContractlessStandardResolver.Instance);

    public object Deserialize(Type type, ReadOnlyMemory<byte> buffer)
    {
        return MessagePackSerializer.Deserialize<BotChatPhotoEvent>(buffer, Options)!;
    }

    public BotChatPhotoEvent Deserialize(ReadOnlyMemory<byte> buffer)
    {
        return MessagePackSerializer.Deserialize<BotChatPhotoEvent>(buffer, Options);
    }

    public void Serialize(IBufferWriter<byte> writer, BotChatPhotoEvent value)
    {
        MessagePackSerializer.Serialize(writer, value, Options);
    }
}
