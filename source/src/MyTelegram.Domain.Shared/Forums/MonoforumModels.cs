namespace MyTelegram.Domain.Shared.Forums;

/// <summary>
/// Monoforum - special forum for direct messages to channels
/// </summary>
public class Monoforum
{
    public long Id { get; set; } // Changed from string to long - this is the Channel ID of the monoforum
    public long ChannelId { get; set; } // The original channel ID that this monoforum is linked to
    public long CreatorId { get; set; }
    public bool IsMonoforum { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public MonoforumSettings Settings { get; set; } = new();
    public List<MonoforumTopic> Topics { get; set; } = new();
    public MonoforumStatistics Statistics { get; set; } = new();
    public bool IsActive { get; set; }
    public string? Description { get; set; }
}

/// <summary>
/// Settings for monoforum behavior
/// </summary>
public class MonoforumSettings
{
    public bool AllowPublicTopics { get; set; } = false; // Only private topics by default
    public bool RequireApprovalForTopics { get; set; } = true;
    public bool EnableModeration { get; set; } = true;
    public bool AutoModerateMessages { get; set; }
    public int MaxTopicsPerUser { get; set; } = 3;
    public int MaxMessageLength { get; set; } = 4096;
    public bool AllowMedia { get; set; } = true;
    public bool AllowLinks { get; set; } = false;
    public bool AllowForwards { get; set; } = false;
    public bool EnableTopicExpiration { get; set; }
    public int TopicExpirationDays { get; set; } = 30;
    public bool EnableReadReceipts { get; set; } = false;
    public bool AllowAnonymousMessages { get; set; } = true;
    public List<string> RestrictedWords { get; set; } = new();
    public List<string> BannedPhrases { get; set; } = new();
    public MonoforumModerationRules ModerationRules { get; set; } = new();
    public bool EnableStatistics { get; set; } = true;
    public string? WelcomeMessage { get; set; }
    public string? Guidelines { get; set; }
    public bool AllowAnonymous { get; set; }
    public bool RequiresApproval { get; set; }
    public List<string> AllowedUserRoles { get; set; } = new();
}

/// <summary>
/// Moderation rules for monoforum
/// </summary>
public class MonoforumModerationRules
{
    public bool FilterProfanity { get; set; } = true;
    public bool FilterSpam { get; set; } = true;
    public bool RequireVerification { get; set; }
    public bool EnableSlowMode { get; set; }
    public int SlowModeDelaySeconds { get; set; } = 30;
    public bool LimitMessagesPerDay { get; set; }
    public int MaxMessagesPerDay { get; set; } = 10;
    public bool BlockSuspiciousUsers { get; set; }
    public bool RequireAccountAge { get; set; }
    public int MinAccountAgeDays { get; set; } = 7;
    public bool RequirePhoneNumber { get; set; } = true;
    public List<string> TrustedUserTypes { get; set; } = new(); // Premium, verified, etc.
}

/// <summary>
/// Individual topic within monoforum
/// </summary>
public class MonoforumTopic
{
    public string Id { get; set; } = string.Empty;
    public long ChannelId { get; set; }
    public long CreatorId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public DateTime? LastActivityAt { get; set; }
    public MonoforumTopicStatus Status { get; set; }
    public bool IsPublic { get; set; } = false;
    public bool IsPinned { get; set; }
    public bool IsClosed { get; set; }
    public DateTime? ClosedAt { get; set; }
    public long ClosedBy { get; set; }
    public string? CloseReason { get; set; }
    public int MessageCount { get; set; }
    public int ParticipantCount { get; set; }
    public List<MonoforumParticipant> Participants { get; set; } = new();
    public List<MonoforumMessage> Messages { get; set; } = new();
    public MonoforumTopicStatistics Statistics { get; set; } = new();
    public string? Tags { get; set; }
    public bool RequiresApproval { get; set; }
    public DateTime? ApprovedAt { get; set; }
    public long ApprovedBy { get; set; }
    public bool IsReported { get; set; }
    public int ReportCount { get; set; }
    public DateTime? ExpiresAt { get; set; }
}

public enum MonoforumTopicStatus
{
    Pending,
    Active,
    Closed,
    Archived,
    Deleted,
    Reported,
    UnderReview
}

/// <summary>
/// Participant in monoforum topic
/// </summary>
public class MonoforumParticipant
{
    public string Id { get; set; } = string.Empty;
    public long TopicId { get; set; }
    public long UserId { get; set; }
    public DateTime JoinedAt { get; set; }
    public MonoforumParticipantRole Role { get; set; }
    public bool IsActive { get; set; }
    public DateTime? LeftAt { get; set; }
    public bool CanPost { get; set; }
    public bool CanModerate { get; set; }
    public bool IsAnonymous { get; set; }
    public DateTime? LastActiveAt { get; set; }
    public int MessageCount { get; set; }
    public bool IsMuted { get; set; }
    public DateTime? MutedUntil { get; set; }
    public string? MuteReason { get; set; }
}

public enum MonoforumParticipantRole
{
    Creator,
    Admin,
    Moderator,
    Member,
    ReadOnly
}

/// <summary>
/// Message within monoforum topic
/// </summary>
public class MonoforumMessage
{
    public string Id { get; set; } = string.Empty;
    public long TopicId { get; set; }
    public long SenderId { get; set; }
    public string Content { get; set; } = string.Empty;
    public MonoforumMessageType Type { get; set; }
    public DateTime SentAt { get; set; }
    public DateTime EditedAt { get; set; }
    public bool IsEdited { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
    public long DeletedBy { get; set; }
    public string? DeletionReason { get; set; }
    public long ReplyToMessageId { get; set; }
    public bool IsAnonymous { get; set; }
    public List<MonoforumMediaAttachment> MediaAttachments { get; set; } = new();
    public int ReactionCount { get; set; }
    public List<MonoforumReaction> Reactions { get; set; } = new();
    public bool IsReported { get; set; }
    public int ReportCount { get; set; }
    public MonoforumMessageStatus Status { get; set; }
}

public enum MonoforumMessageType
{
    Text,
    Photo,
    Video,
    Audio,
    Document,
    Sticker,
    Poll,
    Location,
    Contact
}

public enum MonoforumMessageStatus
{
    Pending,
    Approved,
    Rejected,
    Deleted,
    Reported
}

/// <summary>
/// Media attachment for monoforum message
/// </summary>
public class MonoforumMediaAttachment
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
/// Reaction to monoforum message
/// </summary>
public class MonoforumReaction
{
    public string Id { get; set; } = string.Empty;
    public string MessageId { get; set; } = string.Empty;
    public long UserId { get; set; }
    public string Emoji { get; set; } = string.Empty;
    public DateTime ReactedAt { get; set; }
    public bool IsActive { get; set; }
}

/// <summary>
/// Statistics for monoforum
/// </summary>
public class MonoforumStatistics
{
    public long ChannelId { get; set; }
    public int TotalTopics { get; set; }
    public int ActiveTopics { get; set; }
    public int PendingTopics { get; set; }
    public int TotalMessages { get; set; }
    public int TotalParticipants { get; set; }
    public int ActiveParticipants { get; set; }
    public int MessagesToday { get; set; }
    public int MessagesThisWeek { get; set; }
    public int MessagesThisMonth { get; set; }
    public int TopicsCreatedToday { get; set; }
    public int TopicsCreatedThisWeek { get; set; }
    public int TopicsCreatedThisMonth { get; set; }
    public double AverageMessagesPerTopic { get; set; }
    public double AverageTopicsPerUser { get; set; }
    public DateTime PeriodStart { get; set; }
    public DateTime PeriodEnd { get; set; }
    public List<DailyMonoforumStats> DailyStats { get; set; } = new();
}

/// <summary>
/// Statistics for individual monoforum topic
/// </summary>
public class MonoforumTopicStatistics
{
    public string TopicId { get; set; } = string.Empty;
    public int MessageCount { get; set; }
    public int ParticipantCount { get; set; }
    public int Views { get; set; }
    public int UniqueViews { get; set; }
    public DateTime FirstMessageDate { get; set; }
    public DateTime LastMessageDate { get; set; }
    public double AverageMessagesPerDay { get; set; }
    public List<MonoforumTopParticipant> TopParticipants { get; set; } = new();
}

public class DailyMonoforumStats
{
    public DateTime Date { get; set; }
    public int TopicsCreated { get; set; }
    public int MessagesSent { get; set; }
    public int NewParticipants { get; set; }
    public int ActiveTopics { get; set; }
    public int ActiveParticipants { get; set; }
}

/// <summary>
/// Top participants in monoforum topic
/// </summary>
public class MonoforumTopParticipant
{
    public long UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public int MessageCount { get; set; }
    public DateTime LastActiveDate { get; set; }
    public MonoforumParticipantRole Role { get; set; }
}

/// <summary>
/// Monoforum topic creation request
/// </summary>
public class MonoforumTopicRequest
{
    public long ChannelId { get; set; }
    public long CreatorId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsPublic { get; set; } = false;
    public bool IsAnonymous { get; set; } = false;
    public string? Tags { get; set; }
    public List<long> InitialParticipants { get; set; } = new();
}

/// <summary>
/// Monoforum topic creation result
/// </summary>
public class MonoforumTopicResult
{
    public bool Success { get; set; }
    public string TopicId { get; set; } = string.Empty;
    public MonoforumTopic? Topic { get; set; }
    public string? ErrorMessage { get; set; }
    public bool RequiresApproval { get; set; }
    public bool CreatedImmediately { get; set; }
}

/// <summary>
/// Monoforum moderation action
/// </summary>
public class MonoforumModerationAction
{
    public string Id { get; set; } = string.Empty;
    public long ChannelId { get; set; }
    public long ModeratorId { get; set; }
    public long TargetUserId { get; set; }
    public string? TopicId { get; set; }
    public string? MessageId { get; set; }
    public MonoforumModerationType Type { get; set; }
    public string? Reason { get; set; }
    public DateTime PerformedAt { get; set; }
    public bool IsActive { get; set; }
    public DateTime? ExpiresAt { get; set; }
    public bool WasAppealed { get; set; }
    public DateTime? AppealedAt { get; set; }
    public string? AppealResult { get; set; }
}

public enum MonoforumModerationType
{
    DeleteMessage,
    DeleteTopic,
    MuteUser,
    BanUser,
    CloseTopic,
    WarnUser,
    RestrictUser,
    ApproveTopic,
    RejectTopic
}
