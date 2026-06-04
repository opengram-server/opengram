namespace MyTelegram.ReadModel.Impl;

public class EmojiGroupReadModel : IEmojiGroupReadModel
{
    public string Id { get; private set; } = null!;
    public long Version { get; set; }

    public string CategoryType { get; private set; } = string.Empty;
    public List<EmojiGroup> Groups { get; private set; } = new();
    public long Hash { get; private set; }
    public DateTime UpdatedAt { get; private set; }
}
