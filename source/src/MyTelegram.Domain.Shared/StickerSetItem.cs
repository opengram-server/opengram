namespace MyTelegram;

public record StickerSetItem(long Id, long AccessHash, string Title, string ShortName, bool Masks,
    bool Emojis, bool TextColor, bool ChannelEmojiStatus, List<IPhotoSize>? Thumbs,
    int? ThumbDcId, int? ThumbVersion, long? ThumbDocumentId, int Count,
    List<StickerPackItem> Packs, List<StickerKeywordItem> Keywords, List<TDocument> Documents, StickerSetType StickerSetType);