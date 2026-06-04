namespace MyTelegram.Queries;

/// <summary>
/// Query для получения кастомных эмодзи по списку document_id
/// </summary>
public class GetCustomEmojiDocumentsQuery : IQuery<IReadOnlyList<ICustomEmojiReadModel>>
{
    public GetCustomEmojiDocumentsQuery(List<long> documentIds, long requesterId)
    {
        DocumentIds = documentIds;
        RequesterId = requesterId;
    }

    public List<long> DocumentIds { get; }
    public long RequesterId { get; }
}

/// <summary>
/// Query для поиска кастомных эмодзи по обычному emoji
/// </summary>
public class SearchCustomEmojiQuery : IQuery<IReadOnlyList<ICustomEmojiReadModel>>
{
    public SearchCustomEmojiQuery(string emoticon, long requesterId, bool isPremium, long hash)
    {
        Emoticon = emoticon;
        RequesterId = requesterId;
        IsPremium = isPremium;
        Hash = hash;
    }

    public string Emoticon { get; }
    public long RequesterId { get; }
    public bool IsPremium { get; }
    public long Hash { get; }
}

/// <summary>
/// Query для получения стикерсета с кастомными эмодзи
/// </summary>
public class GetCustomEmojiStickerSetQuery : IQuery<IStickerSetReadModel?>
{
    public GetCustomEmojiStickerSetQuery(long stickerSetId)
    {
        StickerSetId = stickerSetId;
    }

    public long StickerSetId { get; }
}

/// <summary>
/// Query для получения категорий эмодзи
/// </summary>
public class GetEmojiGroupsQuery : IQuery<IEmojiGroupReadModel?>
{
    public GetEmojiGroupsQuery(string categoryType, long hash)
    {
        CategoryType = categoryType;
        Hash = hash;
    }

    public string CategoryType { get; }
    public long Hash { get; }
}

/// <summary>
/// Query для получения emoji packs (группировок по emoji)
/// </summary>
public class GetEmojiPacksQuery : IQuery<IReadOnlyList<IEmojiPackReadModel>>
{
    public GetEmojiPacksQuery(long stickerSetId)
    {
        StickerSetId = stickerSetId;
    }

    public long StickerSetId { get; }
}

/// <summary>
/// Query для получения ключевых слов
/// </summary>
public class GetEmojiKeywordsQuery : IQuery<IReadOnlyList<IEmojiKeywordReadModel>>
{
    public GetEmojiKeywordsQuery(List<long> documentIds)
    {
        DocumentIds = documentIds;
    }

    public List<long> DocumentIds { get; }
}
