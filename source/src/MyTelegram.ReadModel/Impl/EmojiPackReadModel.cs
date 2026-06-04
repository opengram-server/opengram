namespace MyTelegram.ReadModel.Impl;

public class EmojiPackReadModel : IEmojiPackReadModel
{
    public string Id { get; private set; } = null!;
    public long Version { get; set; }

    public long StickerSetId { get; private set; }
    public string Emoticon { get; private set; } = string.Empty;
    public List<long> DocumentIds { get; private set; } = new();
}
