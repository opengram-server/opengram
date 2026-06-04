namespace MyTelegram.ReadModel.Interfaces;

/// <summary>
/// Read model for scheduled messages
/// Stores information about messages that are scheduled to be sent at a future time
/// </summary>
public interface IScheduledMessageReadModel : IReadModel
{
    /// <summary>
    /// Unique identifier for the scheduled message
    /// Format: {OwnerPeerId}_{ScheduleMessageId}
    /// </summary>
    string Id { get; }
    
    /// <summary>
    /// Owner peer ID (user or channel who owns this scheduled message)
    /// </summary>
    long OwnerPeerId { get; }
    
    /// <summary>
    /// Scheduled message ID (unique within owner peer)
    /// </summary>
    int ScheduleMessageId { get; }
    
    /// <summary>
    /// Target peer where message will be sent
    /// </summary>
    Peer ToPeer { get; }
    
    /// <summary>
    /// Sender user ID
    /// </summary>
    long SenderUserId { get; }
    
    /// <summary>
    /// Sender peer (can be user or channel for sendAs)
    /// </summary>
    Peer SenderPeer { get; }
    
    /// <summary>
    /// Message text content
    /// </summary>
    string Message { get; }
    
    /// <summary>
    /// Unix timestamp when message was created
    /// </summary>
    int Date { get; }
    
    /// <summary>
    /// Unix timestamp when message should be sent
    /// </summary>
    int ScheduleDate { get; }
    
    /// <summary>
    /// Random ID from client
    /// </summary>
    long RandomId { get; }
    
    /// <summary>
    /// Message entities (formatting, mentions, etc.)
    /// </summary>
    TVector<IMessageEntity>? Entities { get; }
    
    /// <summary>
    /// Message media (photo, video, document, etc.)
    /// </summary>
    IMessageMedia? Media { get; }
    
    /// <summary>
    /// Reply information
    /// </summary>
    IInputReplyTo? ReplyTo { get; }
    
    /// <summary>
    /// Reply markup (inline keyboard, etc.)
    /// </summary>
    IReplyMarkup? ReplyMarkup { get; }
    
    /// <summary>
    /// Send as peer (for channels with multiple send options)
    /// </summary>
    Peer? SendAs { get; }
    
    /// <summary>
    /// Silent message flag
    /// </summary>
    bool Silent { get; }
    
    /// <summary>
    /// Message effect ID
    /// </summary>
    long? Effect { get; }
    
    /// <summary>
    /// Invert media flag
    /// </summary>
    bool InvertMedia { get; }
    
    /// <summary>
    /// Grouped message ID (for albums)
    /// </summary>
    long? GroupedId { get; }
    
    /// <summary>
    /// Message type
    /// </summary>
    MessageType MessageType { get; }
    
    /// <summary>
    /// Send message type
    /// </summary>
    SendMessageType SendMessageType { get; }
    
    /// <summary>
    /// Whether message has been sent (for tracking delivery)
    /// </summary>
    bool IsSent { get; }
    
    /// <summary>
    /// Actual message ID after sending (null if not sent yet)
    /// </summary>
    int? ActualMessageId { get; }
    
    /// <summary>
    /// When message was actually sent (null if not sent yet)
    /// </summary>
    int? SentDate { get; }
}
