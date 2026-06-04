using MyTelegram.Domain.Aggregates.ChatImport;

namespace MyTelegram.Domain.Aggregates.ChatImport;

[JsonConverter(typeof(SystemTextJsonSingleValueObjectConverter<ChatImportId>))]
public class ChatImportId : Identity<ChatImportId>
{
    public ChatImportId(string value) : base(value)
    {
    }

    public static ChatImportId Create(long chatId)
    {
        return new ChatImportId($"chatimport-{chatId}");
    }
}
