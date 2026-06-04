using System.IO.Compression;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace MyTelegram.Services.CustomEmoji;

/// <summary>
/// Реализация сервиса для работы с кастомными эмодзи
/// </summary>
public class CustomEmojiService : ICustomEmojiService
{
    private readonly ILogger<CustomEmojiService> _logger;
    private readonly string _storageBasePath;
    private const int MaxTgsSize = 64 * 1024; // 64 KB
    private const int ExpectedWidth = 512;
    private const int ExpectedHeight = 512;
    private const int ExpectedFps = 60;
    private const double MaxDuration = 3.0; // seconds

    public CustomEmojiService(
        ILogger<CustomEmojiService> logger,
        IConfiguration configuration)
    {
        _logger = logger;
        _storageBasePath = configuration["CustomEmoji:StoragePath"] 
            ?? Path.Combine(Directory.GetCurrentDirectory(), "files", "custom_emoji");

        // Создаем директорию если не существует
        Directory.CreateDirectory(_storageBasePath);
    }

    public async Task<long> UploadCustomEmojiAsync(
        Stream fileStream,
        string alt,
        long stickerSetId,
        bool isFree,
        bool hasTextColor,
        long uploaderId,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "Uploading custom emoji by user {UserId} to stickerset {StickerSetId}",
            uploaderId,
            stickerSetId);

        // Валидация файла
        var validationResult = await ValidateTgsFileAsync(fileStream, cancellationToken);
        if (!validationResult.IsValid)
        {
            _logger.LogWarning(
                "TGS validation failed: {Errors}",
                string.Join(", ", validationResult.Errors));
            throw new InvalidOperationException(
                $"Invalid TGS file: {string.Join(", ", validationResult.Errors)}");
        }

        // Генерируем document_id
        var documentId = GenerateDocumentId();

        // Сохраняем файл
        var filePath = GetFilePath(documentId);
        fileStream.Position = 0;

        await using (var fileOutput = File.Create(filePath))
        {
            await fileStream.CopyToAsync(fileOutput, cancellationToken);
        }

        _logger.LogInformation(
            "Custom emoji uploaded successfully: documentId={DocumentId}, size={Size}",
            documentId,
            validationResult.Metadata?.CompressedSize);

        // Здесь должно быть сохранение в БД через event sourcing
        // Пример: await PublishDocumentCreatedEventAsync(...)

        return documentId;
    }

    public async Task<Stream?> GetCustomEmojiFileAsync(
        long documentId,
        CancellationToken cancellationToken = default)
    {
        var filePath = GetFilePath(documentId);

        if (!File.Exists(filePath))
        {
            _logger.LogWarning("Custom emoji file not found: {DocumentId}", documentId);
            return null;
        }

        var fileStream = new FileStream(
            filePath,
            FileMode.Open,
            FileAccess.Read,
            FileShare.Read,
            bufferSize: 4096,
            useAsync: true);

        return fileStream;
    }

    public async Task<TgsValidationResult> ValidateTgsFileAsync(
        Stream fileStream,
        CancellationToken cancellationToken = default)
    {
        var result = new TgsValidationResult { IsValid = true };

        try
        {
            // Проверяем размер сжатого файла
            var compressedSize = fileStream.Length;
            if (compressedSize > MaxTgsSize)
            {
                result.Errors.Add($"File size {compressedSize} exceeds maximum {MaxTgsSize} bytes");
                result.IsValid = false;
            }

            fileStream.Position = 0;

            // Разархивируем gzip
            using var gzipStream = new GZipStream(fileStream, CompressionMode.Decompress, leaveOpen: true);
            using var memoryStream = new MemoryStream();
            await gzipStream.CopyToAsync(memoryStream, cancellationToken);

            var uncompressedSize = memoryStream.Length;
            memoryStream.Position = 0;

            // Парсим JSON
            var jsonDoc = await JsonDocument.ParseAsync(memoryStream, cancellationToken: cancellationToken);
            var root = jsonDoc.RootElement;

            // Проверяем наличие обязательного поля "tgs"
            if (!root.TryGetProperty("tgs", out var tgsProperty) || tgsProperty.GetInt32() != 1)
            {
                result.Errors.Add("Missing or invalid 'tgs' field");
                result.IsValid = false;
            }

            // Проверяем размеры
            if (root.TryGetProperty("w", out var widthProp) && widthProp.GetInt32() != ExpectedWidth)
            {
                result.Errors.Add($"Width must be {ExpectedWidth}px");
                result.IsValid = false;
            }

            if (root.TryGetProperty("h", out var heightProp) && heightProp.GetInt32() != ExpectedHeight)
            {
                result.Errors.Add($"Height must be {ExpectedHeight}px");
                result.IsValid = false;
            }

            // Проверяем FPS
            if (root.TryGetProperty("fr", out var fpsProp) && fpsProp.GetInt32() != ExpectedFps)
            {
                result.Errors.Add($"Frame rate must be {ExpectedFps} FPS");
                result.IsValid = false;
            }

            // Проверяем длительность
            if (root.TryGetProperty("ip", out var inPoint) && 
                root.TryGetProperty("op", out var outPoint) &&
                root.TryGetProperty("fr", out var frameRate))
            {
                var duration = (outPoint.GetDouble() - inPoint.GetDouble()) / frameRate.GetDouble();
                if (duration > MaxDuration)
                {
                    result.Errors.Add($"Duration {duration:F2}s exceeds maximum {MaxDuration}s");
                    result.IsValid = false;
                }

                result.Metadata = new TgsMetadata
                {
                    Width = root.TryGetProperty("w", out var w) ? w.GetInt32() : 0,
                    Height = root.TryGetProperty("h", out var h) ? h.GetInt32() : 0,
                    FrameRate = frameRate.GetInt32(),
                    Duration = duration,
                    UncompressedSize = uncompressedSize,
                    CompressedSize = compressedSize,
                    Version = root.TryGetProperty("v", out var v) ? v.GetString() ?? "" : ""
                };
            }

            _logger.LogDebug(
                "TGS validation: IsValid={IsValid}, Errors={Errors}",
                result.IsValid,
                result.Errors.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating TGS file");
            result.IsValid = false;
            result.Errors.Add($"Validation error: {ex.Message}");
        }

        return result;
    }

    public async Task<bool> DeleteCustomEmojiAsync(
        long documentId,
        CancellationToken cancellationToken = default)
    {
        var filePath = GetFilePath(documentId);

        if (!File.Exists(filePath))
        {
            _logger.LogWarning("Custom emoji file not found for deletion: {DocumentId}", documentId);
            return false;
        }

        try
        {
            File.Delete(filePath);
            _logger.LogInformation("Custom emoji deleted: {DocumentId}", documentId);

            // Здесь должно быть удаление из БД через event sourcing

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting custom emoji {DocumentId}", documentId);
            return false;
        }
    }

    public Task<CustomEmojiStats> GetEmojiStatsAsync(
        long documentId,
        CancellationToken cancellationToken = default)
    {
        // Здесь должен быть запрос к БД
        // Пока возвращаем заглушку
        return Task.FromResult(new CustomEmojiStats
        {
            DocumentId = documentId,
            UsageCount = 0,
            CreatedAt = DateTime.UtcNow,
            LastUsedAt = DateTime.UtcNow
        });
    }

    public Task IncrementUsageCountAsync(
        long documentId,
        CancellationToken cancellationToken = default)
    {
        // Здесь должно быть обновление счетчика в БД
        _logger.LogDebug("Incrementing usage count for custom emoji {DocumentId}", documentId);
        return Task.CompletedTask;
    }

    private long GenerateDocumentId()
    {
        // Генерируем уникальный document_id
        // В production можно использовать distributed ID generator (Snowflake, etc.)
        var timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        var random = Random.Shared.Next(1000, 9999);
        return timestamp * 10000 + random;
    }

    private string GetFilePath(long documentId)
    {
        // Распределяем файлы по поддиректориям для лучшей производительности ФС
        var subDir = (documentId % 1000).ToString("D3");
        var dirPath = Path.Combine(_storageBasePath, subDir);
        Directory.CreateDirectory(dirPath);

        return Path.Combine(dirPath, $"{documentId}.tgs");
    }

    private string CalculateMd5(Stream stream)
    {
        using var md5 = MD5.Create();
        var hash = md5.ComputeHash(stream);
        return Convert.ToHexString(hash).ToLowerInvariant();
    }
}
