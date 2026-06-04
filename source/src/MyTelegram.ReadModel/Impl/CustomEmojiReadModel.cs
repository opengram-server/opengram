namespace MyTelegram.ReadModel.Impl;

/// <summary>
/// Реализация ReadModel для кастомных эмодзи
/// </summary>
public class CustomEmojiReadModel : ICustomEmojiReadModel,
    IAmReadModelFor<DocumentAggregate, DocumentId, EmptyDocumentEvent>
{
    public string Id { get; private set; } = null!;
    public long Version { get; set; }

    public long DocumentId { get; private set; }
    public long AccessHash { get; private set; }
    public ReadOnlyMemory<byte> FileReference { get; private set; }
    public int DcId { get; private set; }
    public int Date { get; private set; }
    public string MimeType { get; private set; } = string.Empty;
    public long Size { get; private set; }
    public string FilePath { get; private set; } = string.Empty;
    public string? Md5CheckSum { get; private set; }
    public bool IsFree { get; private set; }
    public bool HasTextColor { get; private set; }
    public string Alt { get; private set; } = string.Empty;
    public long StickerSetId { get; private set; }
    public long? CreatorId { get; private set; }
    public List<PhotoSize>? Thumbs { get; private set; }
    public List<VideoSize>? VideoThumbs { get; private set; }
    public long UsageCount { get; private set; }
    public bool PremiumOnly { get; private set; }

    public Task ApplyAsync(
        IReadModelContext context,
        IDomainEvent<DocumentAggregate, DocumentId, EmptyDocumentEvent> domainEvent,
        CancellationToken cancellationToken)
    {
        // Обработка событий будет добавлена позже
        return Task.CompletedTask;
    }
}
