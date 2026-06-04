using MyTelegram.ReadModel.Interfaces;

namespace MyTelegram.ReadModel.Impl;

/// <summary>
/// ReadModel для sticker set (включая custom emoji packs)
/// Query-only model, populated by admin panel
/// Collection name: ReadModel-StickerSetReadModel
/// </summary>
public class StickerSetReadModel : IStickerSetReadModel
{
    public long StickerSetId { get; set; }
    public long AccessHash { get; set; }
    public string ShortName { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public StickerSetType StickerSetType { get; set; }
    public bool Masks { get; set; }
    public bool Emojis { get; set; }
    public bool TextColor { get; set; }
    public bool ChannelEmojiStatus { get; set; }
    public List<PhotoSize>? Thumbs { get; set; }
    public int? ThumbVersion { get; set; }
    public long? ThumbDocumentId { get; set; }
    public int Count { get; set; }

    public List<StickerPackItem> Packs { get; set; } = new();
    public List<StickerKeywordItem> Keywords { get; set; } = new();
    public List<long> StickerDocumentIds { get; set; } = new();
    public List<long> Covers { get; set; } = new();
    
    // Featured stickers
    public bool IsFeatured { get; set; }
    public int FeaturedOrder { get; set; }  // Порядок отображения в featured списке
}
