namespace MyTelegram.Domain.Aggregates.ChatTheme;

[JsonConverter(typeof(SingleValueObjectConverter))]
public class ChatThemeId : Identity<ChatThemeId>
{
    public ChatThemeId(string value) : base(value)
    {
    }
}
