namespace MyTelegram.ReadModel.Interfaces;

public interface IStickerSetReadModel : IReadModel
{
    long StickerSetId { get; }
    long AccessHash { get; }
    string ShortName { get; }
    string Title { get; }
    StickerSetType StickerSetType { get; }
    bool Masks { get; }
    bool Emojis { get; }
    bool TextColor { get; }
    bool ChannelEmojiStatus { get; }
    List<PhotoSize>? Thumbs { get; }
    int? ThumbVersion { get; }
    long? ThumbDocumentId { get; }
    int Count { get; }

    List<StickerPackItem> Packs { get; }
    List<StickerKeywordItem> Keywords { get; }
    List<long> StickerDocumentIds { get; }
    List<long> Covers { get; }
}