namespace MyTelegram.ReadModel.Impl;

/// <summary>
/// ReadModel for storing bot updates queue (for long polling)
/// Updates are kept for 24 hours as per Telegram Bot API specification
/// </summary>
public class BotUpdateQueueReadModel : IBotUpdateQueueReadModel
{
    public BotUpdateQueueReadModel()
    {
    }

    public BotUpdateQueueReadModel(
        long botUserId,
        long updateId,
        string updateJson,
        string updateType)
    {
        Id = $"botupdate-{botUserId}-{updateId}";
        BotUserId = botUserId;
        UpdateId = updateId;
        UpdateJson = updateJson;
        UpdateType = updateType;
        CreatedAt = DateTime.UtcNow;
        ExpiresAt = DateTime.UtcNow.AddHours(24);
        IsDelivered = false;
    }

    public string Id { get; set; } = default!;
    public long? Version { get; set; }
    public long BotUserId { get; set; }
    public long UpdateId { get; set; }
    public string UpdateJson { get; set; } = default!;
    public string UpdateType { get; set; } = default!;
    public DateTime CreatedAt { get; set; }
    public DateTime ExpiresAt { get; set; }
    public bool IsDelivered { get; set; }
    public DateTime? DeliveredAt { get; set; }
}
