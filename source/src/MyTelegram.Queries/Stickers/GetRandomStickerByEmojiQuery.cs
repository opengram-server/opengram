using MyTelegram.Schema;

namespace MyTelegram.Queries;

public class GetRandomStickerByEmojiQuery(string emoji) : IQuery<long?>
{
    public string Emoji { get; } = emoji;
}
