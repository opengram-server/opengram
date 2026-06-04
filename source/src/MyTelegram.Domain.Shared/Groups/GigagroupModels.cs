namespace MyTelegram.Domain.Shared.Groups;

/// <summary>
/// Gigagroup - a special type of supergroup for massive communities (>200k members)
/// </summary>
public class Gigagroup
{
    public string Id { get; set; } = string.Empty;
    public long ChannelId { get; set; }
    public long CreatorId { get; set; }
    public bool IsGigagroup { get; set; }
    public DateTime ConvertedAt { get; set; }
    public long ConvertedFromSupergroupId { get; set; }
    public GigagroupSettings Settings { get; set; } = new();
    public GigagroupStatistics Statistics { get; set; } = new();
    public List<GigagroupAdmin> Admins { get; set; } = new();
    public List<GigagroupModerator> Moderators { get; set; } = new();
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

/// <summary>
/// Settings for gigagroup behavior and restrictions
/// </summary>
public class GigagroupSettings
{
    public bool AllowAdminsToWrite { get; set; } = true;
    public bool AllowVoiceChatParticipants { get; set; } = false; // Muted by default
    public bool EnableAntiSpam { get; set; } = true;
    public bool EnableSlowMode { get; set; }
    public int SlowModeDelaySeconds { get; set; } = 30;
    public bool RequireVerification { get; set; }
    public bool EnableRestrictedInvites { get; set; } = true; // No direct invites
    public bool EnableAdminApprovalForJoinRequests { get; set; } = true;
    public int MaxMessageLength { get; set; } = 4096;
    public bool AllowMedia { get; set; } = true;
    public bool AllowLinks { get; set; } = false;
    public bool AllowForwards { get; set; } = false;
    public List<string> RestrictedMediaTypes { get; set; } = new();
    public TimeZoneInfo Timezone { get; set; } = TimeZoneInfo.Utc;
    public string? WelcomeMessage { get; set; }
    public List<GigagroupRule> Rules { get; set; } = new();
    public bool EnableStatistics { get; set; } = true;
    public bool EnableContentModeration { get; set; } = true;
}

/// <summary>
/// Custom rules for gigagroup
/// </summary>
public class GigagroupRule
{
    public string Id { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public long CreatedBy { get; set; }
    public RuleType Type { get; set; }
}

public enum RuleType
{
    ContentRestriction,
    BehaviorGuideline,
    PostingGuideline,
    VoiceChatRule,
    Custom
}

/// <summary>
/// Statistics for gigagroup performance
/// </summary>
public class GigagroupStatistics
{
    public long ChannelId { get; set; }
    public long TotalMembers { get; set; }
    public long ActiveMembers { get; set; }
    public long AdminCount { get; set; }
    public long ModeratorCount { get; set; }
    public int MessagesToday { get; set; }
    public int MessagesThisWeek { get; set; }
    public int MessagesThisMonth { get; set; }
    public long TotalMessages { get; set; }
    public int VoiceChatParticipants { get; set; }
    public int AverageVoiceChatDuration { get; set; } // Minutes
    public int JoinRequests { get; set; }
    public int PendingJoinRequests { get; set; }
    public int SpammersRemoved { get; set; }
    public int RulesViolations { get; set; }
    public double EngagementRate { get; set; } // Active members / total members
    public DateTime LastActivity { get; set; }
    public List<DailyActivityStats> DailyStats { get; set; } = new();
}

public class DailyActivityStats
{
    public DateTime Date { get; set; }
    public int MessagesSent { get; set; }
    public int ActiveMembers { get; set; }
    public int NewMembers { get; set; }
    public int LeftMembers { get; set; }
    public int VoiceChatParticipants { get; set; }
    public int RuleViolations { get; set; }
}

/// <summary>
/// Gigagroup administrator with special permissions
/// </summary>
public class GigagroupAdmin
{
    public string Id { get; set; } = string.Empty;
    public long UserId { get; set; }
    public long ChannelId { get; set; }
    public DateTime PromotedAt { get; set; }
    public long PromotedBy { get; set; }
    public bool CanPostMessages { get; set; } = true;
    public bool CanManageVoiceChat { get; set; }
    public bool CanManageJoinRequests { get; set; }
    public bool CanManageBans { get; set; }
    public bool CanManageAdmins { get; set; }
    public bool CanManageSettings { get; set; }
    public bool CanManageStatistics { get; set; }
    public bool CanInviteUsers { get; set; } = false; // Restricted by default
    public List<GigagroupPermission> CustomPermissions { get; set; } = new();
    public bool IsActive { get; set; }
    public DateTime? DemotedAt { get; set; }
    public string? DemotionReason { get; set; }
}

/// <summary>
/// Gigagroup moderator with limited permissions
/// </summary>
public class GigagroupModerator
{
    public string Id { get; set; } = string.Empty;
    public long UserId { get; set; }
    public long ChannelId { get; set; }
    public DateTime PromotedAt { get; set; }
    public long PromotedBy { get; set; }
    public bool CanDeleteMessages { get; set; }
    public bool CanBanUsers { get; set; }
    public bool CanManageVoiceChat { get; set; }
    public bool CanApproveJoinRequests { get; set; }
    public List<string> ModeratedTopics { get; set; } = new(); // If forum enabled
    public bool IsActive { get; set; }
    public DateTime? DemotedAt { get; set; }
    public string? DemotionReason { get; set; }
}

/// <summary>
/// Custom permission for gigagroup admin
/// </summary>
public class GigagroupPermission
{
    public string Name { get; set; } = string.Empty;
    public bool IsGranted { get; set; }
    public DateTime? ExpiresAt { get; set; }
    public string? Reason { get; set; }
}

/// <summary>
/// Gigagroup conversion request
/// </summary>
public class GigagroupConversionRequest
{
    public long SupergroupId { get; set; }
    public long RequestedBy { get; set; }
    public string? Reason { get; set; }
    public bool RequiresConfirmation { get; set; } = true;
    public List<long> ConfirmingAdmins { get; set; } = new(); // Admin IDs that must confirm
}

/// <summary>
/// Gigagroup conversion result
/// </summary>
public class GigagroupConversionResult
{
    public bool Success { get; set; }
    public string GigagroupId { get; set; } = string.Empty;
    public DateTime ConvertedAt { get; set; }
    public string? ErrorMessage { get; set; }
    public bool RequiresConfirmation { get; set; }
    public List<long> PendingConfirmations { get; set; } = new();
    public string? ConfirmationToken { get; set; }
}

/// <summary>
/// Gigagroup join request (special handling for restricted invites)
/// </summary>
public class GigagroupJoinRequest
{
    public string Id { get; set; } = string.Empty;
    public long UserId { get; set; }
    public long ChannelId { get; set; }
    public DateTime RequestedAt { get; set; }
    public string? Message { get; set; }
    public bool IsVerified { get; set; }
    public DateTime? VerifiedAt { get; set; }
    public JoinRequestStatus Status { get; set; }
    public long ReviewedBy { get; set; }
    public DateTime? ReviewedAt { get; set; }
    public string? RejectionReason { get; set; }
}

public enum JoinRequestStatus
{
    Pending,
    Approved,
    Rejected,
    Cancelled
}

/// <summary>
/// Gigagroup voice chat settings
/// </summary>
public class GigagroupVoiceChatSettings
{
    public long ChannelId { get; set; }
    public bool Enabled { get; set; }
    public bool DefaultMuted { get; set; } = true; // Muted by default for gigagroups
    public int MaxParticipants { get; set; }
    public bool RequireAdminApproval { get; set; }
    public int AutoMuteOnJoinDelay { get; set; } = 5; // Seconds
    public bool AllowScreenSharing { get; set; }
    public bool AllowRecording { get; set; }
    public List<VoiceChatModerator> Moderators { get; set; } = new();
}

/// <summary>
/// Voice chat moderator for gigagroup
/// </summary>
public class VoiceChatModerator
{
    public long UserId { get; set; }
    public bool CanMute { get; set; }
    public bool CanKick { get; set; }
    public bool CanRaiseHand { get; set; }
    public bool CanManageQueue { get; set; }
    public DateTime AssignedAt { get; set; }
    public long AssignedBy { get; set; }
}

/// <summary>
/// Gigagroup content moderation
/// </summary>
public class GigagroupModeration
{
    public string Id { get; set; } = string.Empty;
    public long ChannelId { get; set; }
    public long ModeratorId { get; set; }
    public long TargetUserId { get; set; }
    public long MessageId { get; set; }
    public ModerationAction Action { get; set; }
    public string? Reason { get; set; }
    public DateTime PerformedAt { get; set; }
    public bool IsActive { get; set; }
    public DateTime? ExpiresAt { get; set; }
    public string? Evidence { get; set; }
    public bool WasAppealed { get; set; }
    public DateTime? AppealedAt { get; set; }
    public string? AppealResult { get; set; }
}

public enum ModerationAction
{
    DeleteMessage,
    Warning,
    Mute,
    Ban,
    Kick,
    RestrictMedia,
    RestrictLinks,
    Custom
}
