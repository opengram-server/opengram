namespace MyTelegram.ReadModel.Interfaces;

/// <summary>
/// ReadModel for storing bot updates queue (for long polling)
/// </summary>
public interface IBotUpdateQueueReadModel : IReadModel
{
    /// <summary>
    /// Unique ID: "botupdate-{botUserId}-{updateId}"
    /// </summary>
    string Id { get; }
    
    /// <summary>
    /// Bot user ID
    /// </summary>
    long BotUserId { get; }
    
    /// <summary>
    /// Sequential update ID
    /// </summary>
    long UpdateId { get; }
    
    /// <summary>
    /// Serialized update JSON (Bot API format)
    /// </summary>
    string UpdateJson { get; }
    
    /// <summary>
    /// Update type (message, edited_message, callback_query, etc.)
    /// </summary>
    string UpdateType { get; }
    
    /// <summary>
    /// When update was created
    /// </summary>
    DateTime CreatedAt { get; }
    
    /// <summary>
    /// When update expires (24 hours from creation)
    /// </summary>
    DateTime ExpiresAt { get; }
    
    /// <summary>
    /// Whether update was delivered to bot
    /// </summary>
    bool IsDelivered { get; }
    
    /// <summary>
    /// When update was delivered
    /// </summary>
    DateTime? DeliveredAt { get; }
}
