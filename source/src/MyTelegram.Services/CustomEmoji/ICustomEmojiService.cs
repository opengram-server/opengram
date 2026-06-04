namespace MyTelegram.Services.CustomEmoji;

/// <summary>
/// Сервис для работы с кастомными эмодзи и TGS файлами
/// </summary>
public interface ICustomEmojiService
{
    /// <summary>
    /// Загрузить TGS файл на сервер
    /// </summary>
    Task<long> UploadCustomEmojiAsync(
        Stream fileStream,
        string alt,
        long stickerSetId,
        bool isFree,
        bool hasTextColor,
        long uploaderId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Получить файл TGS по document_id
    /// </summary>
    Task<Stream?> GetCustomEmojiFileAsync(
        long documentId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Валидировать TGS файл
    /// </summary>
    Task<TgsValidationResult> ValidateTgsFileAsync(
        Stream fileStream,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Удалить кастомный эмодзи
    /// </summary>
    Task<bool> DeleteCustomEmojiAsync(
        long documentId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Получить статистику использования
    /// </summary>
    Task<CustomEmojiStats> GetEmojiStatsAsync(
        long documentId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Обновить счетчик использований
    /// </summary>
    Task IncrementUsageCountAsync(
        long documentId,
        CancellationToken cancellationToken = default);
}

/// <summary>
/// Результат валидации TGS файла
/// </summary>
public class TgsValidationResult
{
    public bool IsValid { get; set; }
    public List<string> Errors { get; set; } = new();
    public TgsMetadata? Metadata { get; set; }
}

/// <summary>
/// Метаданные TGS файла
/// </summary>
public class TgsMetadata
{
    public int Width { get; set; }
    public int Height { get; set; }
    public int FrameRate { get; set; }
    public double Duration { get; set; }
    public long UncompressedSize { get; set; }
    public long CompressedSize { get; set; }
    public string Version { get; set; } = string.Empty;
}

/// <summary>
/// Статистика кастомного эмодзи
/// </summary>
public class CustomEmojiStats
{
    public long DocumentId { get; set; }
    public long UsageCount { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime LastUsedAt { get; set; }
}
