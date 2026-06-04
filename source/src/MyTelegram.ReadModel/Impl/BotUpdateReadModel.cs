using EventFlow.MongoDB.ReadStores.Attributes;
using MyTelegram.Domain.Shared.BotApi;

namespace MyTelegram.ReadModel.Impl;

/// <summary>
/// ReadModel for storing bot updates (for getUpdates method)
/// </summary>
[MongoDbCollectionName("ReadModel-BotUpdateReadModel")]
public class BotUpdateReadModel : IBotUpdateReadModel
{
    public BotUpdateReadModel()
    {
    }

    public BotUpdateReadModel(
        long botUserId,
        long updateId,
        BotApiUpdate update)
    {
        Id = $"bot-update-{botUserId}-{updateId}";
        BotUserId = botUserId;
        UpdateId = updateId;
        Update = update;
        CreatedAt = DateTime.UtcNow;
        IsDelivered = false;
    }

    public string Id { get; set; } = default!;
    public long? Version { get; set; }
    
    /// <summary>
    /// Bot user ID
    /// </summary>
    public long BotUserId { get; set; }
    
    /// <summary>
    /// Update ID (sequential)
    /// </summary>
    public long UpdateId { get; set; }
    
    /// <summary>
    /// Bot API Update object (serialized)
    /// </summary>
    public BotApiUpdate Update { get; set; } = default!;
    
    /// <summary>
    /// Creation timestamp
    /// </summary>
    public DateTime CreatedAt { get; set; }
    
    /// <summary>
    /// Whether update was delivered via getUpdates
    /// </summary>
    public bool IsDelivered { get; set; }
    
    /// <summary>
    /// Delivery timestamp
    /// </summary>
    public DateTime? DeliveredAt { get; set; }
    
    /// <summary>
    /// Update type (message, edited_message, callback_query, etc.)
    /// </summary>
    public string UpdateType { get; set; } = default!;
}
