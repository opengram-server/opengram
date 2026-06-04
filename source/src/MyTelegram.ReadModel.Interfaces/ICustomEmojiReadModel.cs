namespace MyTelegram.ReadModel.Interfaces;

/// <summary>
/// ReadModel для кастомных эмодзи (Custom Emoji)
/// Представляет документ с documentAttributeCustomEmoji
/// </summary>
public interface ICustomEmojiReadModel : IReadModel
{
    /// <summary>
    /// Уникальный ID документа кастомного эмодзи
    /// </summary>
    long DocumentId { get; }

    /// <summary>
    /// Access hash для безопасного доступа
    /// </summary>
    long AccessHash { get; }

    /// <summary>
    /// File reference для получения файла
    /// </summary>
    ReadOnlyMemory<byte> FileReference { get; }

    /// <summary>
    /// ID Data Center, где хранится файл
    /// </summary>
    int DcId { get; }

    /// <summary>
    /// Дата создания (Unix timestamp)
    /// </summary>
    int Date { get; }

    /// <summary>
    /// MIME type: "application/x-tgsticker" для TGS или "video/webm" для video emoji
    /// </summary>
    string MimeType { get; }

    /// <summary>
    /// Размер файла в байтах
    /// </summary>
    long Size { get; }

    /// <summary>
    /// Путь к файлу в локальном хранилище
    /// </summary>
    string FilePath { get; }

    /// <summary>
    /// MD5 чексумма файла
    /// </summary>
    string? Md5CheckSum { get; }

    /// <summary>
    /// Доступен ли эмодзи не-Premium пользователям
    /// </summary>
    bool IsFree { get; }

    /// <summary>
    /// Должен ли менять цвет в зависимости от контекста
    /// </summary>
    bool HasTextColor { get; }

    /// <summary>
    /// Обычный emoji для fallback (UTF-8)
    /// </summary>
    string Alt { get; }

    /// <summary>
    /// ID стикерсета, к которому принадлежит эмодзи
    /// </summary>
    long StickerSetId { get; }

    /// <summary>
    /// ID создателя
    /// </summary>
    long? CreatorId { get; }

    /// <summary>
    /// Thumbnails (превью изображения)
    /// </summary>
    List<PhotoSize>? Thumbs { get; }

    /// <summary>
    /// Video thumbnails (для video emoji)
    /// </summary>
    List<VideoSize>? VideoThumbs { get; }

    /// <summary>
    /// Счетчик использований (для статистики)
    /// </summary>
    long UsageCount { get; }

    /// <summary>
    /// Доступен только Premium пользователям
    /// </summary>
    bool PremiumOnly { get; }
}
