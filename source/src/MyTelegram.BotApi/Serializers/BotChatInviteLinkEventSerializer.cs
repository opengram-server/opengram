using System.Buffers;
using MyTelegram.Abstractions;
using MyTelegram.Domain.Shared.Events;
using MessagePack;

namespace MyTelegram.BotApi.Serializers;

public class BotChatInviteLinkEventSerializer : IEventDataSerializer<BotChatInviteLinkEvent>
{
    private static readonly MessagePackSerializerOptions Options = MessagePackSerializerOptions.Standard
        .WithResolver(MessagePack.Resolvers.ContractlessStandardResolver.Instance);

    public object Deserialize(Type type, ReadOnlyMemory<byte> buffer)
    {
        return MessagePackSerializer.Deserialize<BotChatInviteLinkEvent>(buffer, Options)!;
    }

    public BotChatInviteLinkEvent Deserialize(ReadOnlyMemory<byte> buffer)
    {
        return MessagePackSerializer.Deserialize<BotChatInviteLinkEvent>(buffer, Options);
    }

    public void Serialize(IBufferWriter<byte> writer, BotChatInviteLinkEvent value)
    {
        MessagePackSerializer.Serialize(writer, value, Options);
    }
}
