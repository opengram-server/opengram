namespace MyTelegram.Domain.Shared.DirectMessages;

/// <summary>
/// Direct Messages topic for channel chat
/// </summary>
public class DirectMessagesTopic
{
    public string Id { get; set; } = string.Empty;
    public long ChannelId { get; set; }
    public long UserId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public DateTime? LastActivityAt { get; set; }
    public DirectMessageTopicStatus Status { get; set; }
    public bool IsActive { get; set; }
    public int MessageCount { get; set; }
    public long LastMessageId { get; set; }
    public DateTime LastMessageDate { get; set; }
    public long LastSenderId { get; set; }
    public string? LastMessagePreview { get; set; }
    public bool IsUnread { get; set; }
    public int UnreadCount { get; set; }
    public bool IsPinned { get; set; }
    public DateTime? PinnedAt { get; set; }
    public DirectMessageSettings Settings { get; set; } = new();
}

/// <summary>
/// Status of direct message topic
/// </summary>
public enum DirectMessageTopicStatus
{
    Active,
    Archived,
    Deleted,
    Blocked,
    Restricted
}

/// <summary>
/// Settings for direct messages
/// </summary>
public class DirectMessageSettings
{
    public bool AllowDirectMessages { get; set; } = true;
    public long PricePerMessage { get; set; } = 0; // Stars per message
    public bool RequireVerification { get; set; }
    public bool AllowMedia { get; set; } = true;
    public bool AllowLinks { get; set; } = false;
    public bool AllowForwards { get; set; } = false;
    public int MaxMessageLength { get; set; } = 4096;
    public bool EnableModeration { get; set; } = true;
    public bool AutoReplyEnabled { get; set; }
    public string? AutoReplyMessage { get; set; }
    public TimeSpan ResponseTime { get; set; } = TimeSpan.FromHours(24);
    public bool RequirePayment { get; set; } = false;
    public List<string> RestrictedWords { get; set; } = new();
    public List<string> AllowedUserTypes { get; set; } = new(); // Premium, verified, etc.
    public int MaxMessagesPerDay { get; set; } = 50;
    public bool EnableSpamFilter { get; set; } = true;
}

/// <summary>
/// Direct message
/// </summary>
public class DirectMessage
{
    public string Id { get; set; } = string.Empty;
    public string TopicId { get; set; } = string.Empty;
    public long ChannelId { get; set; }
    public long SenderId { get; set; }
    public long ReceiverId { get; set; }
    public string Content { get; set; } = string.Empty;
    public DirectMessageType Type { get; set; }
    public DateTime SentAt { get; set; }
    public DateTime EditedAt { get; set; }
    public bool IsEdited { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
    public long DeletedBy { get; set; }
    public long ReplyToMessageId { get; set; }
    public List<DirectMessageMediaAttachment> MediaAttachments { get; set; } = new();
    public bool IsRead { get; set; }
    public DateTime? ReadAt { get; set; }
    public long StarsCost { get; set; }
    public bool IsPaid { get; set; }
    public DirectMessageStatus Status { get; set; }
    public bool IsReported { get; set; }
    public int ReportCount { get; set; }
}

/// <summary>
/// Type of direct message
/// </summary>
public enum DirectMessageType
{
    Text,
    Photo,
    Video,
    Audio,
    Document,
    Sticker,
    Location,
    Contact,
    Voice
}

/// <summary>
/// Status of direct message
/// </summary>
public enum DirectMessageStatus
{
    Pending,
    Sent,
    Delivered,
    Read,
    Failed,
    Deleted,
    Reported
}

/// <summary>
/// Media attachment for direct message
/// </summary>
public class DirectMessageMediaAttachment
{
    public string Id { get; set; } = string.Empty;
    public string MessageId { get; set; } = string.Empty;
    public string FileId { get; set; } = string.Empty;
    public string FileName { get; set; } = string.Empty;
    public string MimeType { get; set; } = string.Empty;
    public long FileSize { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }
    public int Duration { get; set; }
    public string? Thumbnail { get; set; }
    public string? Caption { get; set; }
    public DateTime UploadedAt { get; set; }
}

/// <summary>
/// Service message about direct message price change
/// </summary>
public class DirectMessagePriceChanged
{
    public long ChannelId { get; set; }
    public long OldPrice { get; set; }
    public long NewPrice { get; set; }
    public DateTime ChangedAt { get; set; }
    public long ChangedBy { get; set; }
    public string? Reason { get; set; }
}

/// <summary>
/// Direct message statistics
/// </summary>
public class DirectMessageStatistics
{
    public long ChannelId { get; set; }
    public int TotalTopics { get; set; }
    public int ActiveTopics { get; set; }
    public int TotalMessages { get; set; }
    public int MessagesToday { get; set; }
    public int MessagesThisWeek { get; set; }
    public int MessagesThisMonth { get; set; }
    public long TotalRevenue { get; set; }
    public double AverageResponseTime { get; set; }
    public double SatisfactionRating { get; set; }
    public List<UserDirectMessageStats> UserStats { get; set; } = new();
    public DateTime PeriodStart { get; set; }
    public DateTime PeriodEnd { get; set; }
}

/// <summary>
/// User-specific direct message statistics
/// </summary>
public class UserDirectMessageStats
{
    public long UserId { get; set; }
    public int MessagesSent { get; set; }
    public int MessagesReceived { get; set; }
    public long TotalSpent { get; set; }
    public DateTime LastActivity { get; set; }
    public bool IsActiveUser { get; set; }
}

/// <summary>
/// Direct message creation request
/// </summary>
public class CreateDirectMessageRequest
{
    public string TopicId { get; set; } = string.Empty;
    public long SenderId { get; set; }
    public long ChannelId { get; set; }
    public string Content { get; set; } = string.Empty;
    public DirectMessageType Type { get; set; }
    public List<DirectMessageMediaAttachment> MediaAttachments { get; set; } = new();
    public long ReplyToMessageId { get; set; }
}

/// <summary>
/// Direct message creation result
/// </summary>
public class CreateDirectMessageResult
{
    public bool Success { get; set; }
    public DirectMessage? Message { get; set; }
    public string? ErrorMessage { get; set; }
    public long StarsCharged { get; set; }
    public bool RequiresPayment { get; set; }
    public string PaymentUrl { get; set; } = string.Empty;
    public long MessageId { get; set; }
}

/// <summary>
/// Direct message topic creation request
/// </summary>
public class CreateDirectMessageTopicRequest
{
    public long ChannelId { get; set; }
    public long UserId { get; set; }
    public string? InitialMessage { get; set; }
    public bool PayForInitialMessage { get; set; }
}

/// <summary>
/// Direct message moderation action
/// </summary>
public class DirectMessageModerationAction
{
    public string Id { get; set; } = string.Empty;
    public long ChannelId { get; set; }
    public long ModeratorId { get; set; }
    public long TargetUserId { get; set; }
    public string? MessageId { get; set; }
    public DirectMessageModerationType Type { get; set; }
    public string? Reason { get; set; }
    public DateTime PerformedAt { get; set; }
    public bool IsActive { get; set; }
    public DateTime? ExpiresAt { get; set; }
}

/// <summary>
/// Types of moderation actions for direct messages
/// </summary>
public enum DirectMessageModerationType
{
    DeleteMessage,
    BanUser,
    MuteUser,
    RestrictUser,
    WarnUser,
    CloseTopic,
    ArchiveTopic,
    ReportMessage
}

/// <summary>
/// Direct message pricing model
/// </summary>
public class DirectMessagePricing
{
    public long ChannelId { get; set; }
    public long BasePrice { get; set; }
    public bool DynamicPricing { get; set; }
    public double Multiplier { get; set; } = 1.0;
    public List<PricingTier> Tiers { get; set; } = new();
    public DateTime LastUpdated { get; set; }
    public long UpdatedBy { get; set; }
}

/// <summary>
/// Pricing tier for direct messages
/// </summary>
public class PricingTier
{
    public int MessageCount { get; set; }
    public long Price { get; set; }
    public string? Description { get; set; }
    public bool IsActive { get; set; }
}

/// <summary>
/// Direct message validation result
/// </summary>
public class DirectMessageValidationResult
{
    public bool IsValid { get; set; }
    public string? ErrorMessage { get; set; }
    public List<string> ValidationErrors { get; set; } = new();
    public bool RequiresPayment { get; set; }
    public long RequiredStars { get; set; }
}
