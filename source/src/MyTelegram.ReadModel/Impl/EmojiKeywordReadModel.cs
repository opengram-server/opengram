namespace MyTelegram.ReadModel.Impl;

public class EmojiKeywordReadModel : IEmojiKeywordReadModel
{
    public string Id { get; private set; } = null!;
    public long Version { get; set; }

    public long DocumentId { get; private set; }
    public List<string> Keywords { get; private set; } = new();
}
