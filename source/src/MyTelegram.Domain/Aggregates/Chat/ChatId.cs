using System.Text.Json.Serialization;

namespace MyTelegram.Domain.Aggregates.Chat;

[JsonConverter(typeof(SystemTextJsonSingleValueObjectConverter<ChatId>))]
public class ChatId(string value) : Identity<ChatId>(value)
{
    public static ChatId Create(long chatId)
    {
        return NewDeterministic(GuidFactories.Deterministic.Namespaces.Commands, $"chat-{chatId}");
    }
}
