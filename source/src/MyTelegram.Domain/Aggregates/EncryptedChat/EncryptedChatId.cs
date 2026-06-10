namespace MyTelegram.Domain.Aggregates.EncryptedChat;

[JsonConverter(typeof(SystemTextJsonSingleValueObjectConverter<EncryptedChatId>))]
public class EncryptedChatId(string value) : Identity<EncryptedChatId>(value)
{
    public static EncryptedChatId Create(int chatId)
    {
        return NewDeterministic(GuidFactories.Deterministic.Namespaces.Commands, $"encryptedchat_{chatId}");
    }
}
