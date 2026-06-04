namespace MyTelegram.Messenger.Extensions;

public static class MessageEntityExtensions
{
    public static IMessageEntity? CreateMessageEntityBold(this string text, string subText)
    {
        var startIndex = text.IndexOf(subText, StringComparison.OrdinalIgnoreCase);
        if (startIndex != -1)
        {
            return new TMessageEntityBold
            {
                Offset = startIndex,
                Length = subText.Length
            };
        }

        return null;
    }
}