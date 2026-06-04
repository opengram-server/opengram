namespace MyTelegram.Domain.Aggregates.Messaging;

[JsonConverter(typeof(SystemTextJsonSingleValueObjectConverter<MessageId>))]
public class MessageId(string value) : Identity<MessageId>(value)
{
    public static MessageId Create(long ownerPeerId, int messageId, bool quickReplyMessage = false)
    {
        var name = $"message_{ownerPeerId}_{messageId}";
        if (quickReplyMessage)
        {
            name = $"quick_reply_message_{ownerPeerId}_{messageId}";
        }

        return NewDeterministic(GuidFactories.Deterministic.Namespaces.Commands, name);
    }
}