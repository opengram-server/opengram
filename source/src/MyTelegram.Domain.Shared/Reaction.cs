using System.Text;

namespace MyTelegram;

public record Reaction(
    long UserId,
    string? Emoticon,
    long? CustomEmojiDocumentId,
    int? Date = 0)
{
    public long UserId { get; set; } = UserId;
    public string? Emoticon { get; set; } = Emoticon;
    public long? CustomEmojiDocumentId { get; set; } = CustomEmojiDocumentId;

    public int? Date { get; set; } = Date;

    public long GetReactionId()
    {
        if (CustomEmojiDocumentId.HasValue)
        {
            return CustomEmojiDocumentId.Value;
        }

        if (string.IsNullOrEmpty(Emoticon))
        {
            throw new InvalidOperationException("Emotion and CustomEmojiDocumentId is null");
        }
        var bytes = Encoding.UTF8.GetBytes(Emoticon);
        if (bytes.Length >= 8)
        {
            return BitConverter.ToInt64(bytes);
        }

        var newBytes = new byte[8];
        Buffer.BlockCopy(bytes, 0, newBytes, 0, bytes.Length);

        return BitConverter.ToInt64(newBytes);
    }
}