using System;
using System.Collections.Generic;

namespace MyTelegram.Domain.Shared.SuggestedPosts;

/// <summary>
/// Suggested post for channel monetization
/// </summary>
public class SuggestedPost
{
    public string Id { get; set; } = string.Empty;
    public long ChannelId { get; set; }
    public long SuggestedBy { get; set; }
    public string Content { get; set; } = string.Empty;
    public SuggestedPostType Type { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public SuggestedPostStatus Status { get; set; }
    public SuggestedPostPrice Price { get; set; } = new();
    public List<SuggestedPostMediaAttachment> MediaAttachments { get; set; } = new();
    public List<string> Entities { get; set; } = new(); // Changed from MessageEntity to string
    public long? PublishedMessageId { get; set; }
    public DateTime? PublishedAt { get; set; }
    public long PublishedBy { get; set; }
    public string? RejectionReason { get; set; }
    public DateTime? ReviewedAt { get; set; }
    public long ReviewedBy { get; set; }
    public int ViewCount { get; set; }
    public int LikeCount { get; set; }
    public int ShareCount { get; set; }
    public SuggestedPostStatistics Statistics { get; set; } = new();
    public bool IsPinned { get; set; }
    public DateTime? PinnedAt { get; set; }
    public List<long> Tags { get; set; } = new(); // User IDs mentioned
    public string? Preview { get; set; }
    public bool DisableWebPagePreview { get; set; }
    public bool Silent { get; set; }
    public DateTime? ExpiresAt { get; set; }
}

/// <summary>
/// Type of suggested post
/// </summary>
public enum SuggestedPostType
{
    Text,
    Photo,
    Video,
    Audio,
    Document,
    Link,
    Poll
}

/// <summary>
/// Status of suggested post
/// </summary>
public enum SuggestedPostStatus
{
    Pending,
    Approved,
    Published,
    Rejected,
    Expired,
    Withdrawn,
    PaymentPending,
    Paid,
    Refunded
}

/// <summary>
/// Pricing information for suggested post
/// </summary>
public class SuggestedPostPrice
{
    public long Amount { get; set; } // Stars
    public string Currency { get; set; } = "XTR"; // Telegram Stars
    public DateTime ValidUntil { get; set; }
    public bool IsNegotiable { get; set; }
    public long MinAmount { get; set; }
    public long MaxAmount { get; set; }
    public bool RequiresPayment { get; set; }
}

/// <summary>
/// Media attachment for suggested post
/// </summary>
public class SuggestedPostMediaAttachment
{
    public string Id { get; set; } = string.Empty;
    public string PostId { get; set; } = string.Empty;
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
/// Statistics for suggested post
/// </summary>
public class SuggestedPostStatistics
{
    public string PostId { get; set; } = string.Empty;
    public int Impressions { get; set; }
    public int UniqueViews { get; set; }
    public int Clicks { get; set; }
    public double ClickThroughRate { get; set; }
    public int EngagementTime { get; set; } // Average engagement time in seconds
    public List<DailySuggestedPostStats> DailyStats { get; set; } = new();
    public DateTime PeriodStart { get; set; }
    public DateTime PeriodEnd { get; set; }
}

/// <summary>
/// Daily statistics for suggested post
/// </summary>
public class DailySuggestedPostStats
{
    public DateTime Date { get; set; }
    public int Impressions { get; set; }
    public int Views { get; set; }
    public int Clicks { get; set; }
    public double Ctr { get; set; }
}

/// <summary>
/// Service message about suggested post approval
/// </summary>
public class SuggestedPostApproved
{
    public string PostId { get; set; } = string.Empty;
    public long MessageId { get; set; }
    public long ChannelId { get; set; }
    public long ApprovedBy { get; set; }
    public DateTime ApprovedAt { get; set; }
    public SuggestedPostPrice Price { get; set; } = new();
}

/// <summary>
/// Service message about suggested post approval failure
/// </summary>
public class SuggestedPostApprovalFailed
{
    public string PostId { get; set; } = string.Empty;
    public long MessageId { get; set; }
    public long ChannelId { get; set; }
    public string Reason { get; set; } = string.Empty;
    public DateTime FailedAt { get; set; }
    public SuggestedPostPrice Price { get; set; } = new();
}

/// <summary>
/// Service message about suggested post rejection
/// </summary>
public class SuggestedPostDeclined
{
    public string PostId { get; set; } = string.Empty;
    public long MessageId { get; set; }
    public long ChannelId { get; set; }
    public long DeclinedBy { get; set; }
    public string Reason { get; set; } = string.Empty;
    public DateTime DeclinedAt { get; set; }
}

/// <summary>
/// Service message about successful payment for suggested post
/// </summary>
public class SuggestedPostPaid
{
    public string PostId { get; set; } = string.Empty;
    public long MessageId { get; set; }
    public long ChannelId { get; set; }
    public long PaidBy { get; set; }
    public long Amount { get; set; } // Changed from StarAmount to long
    public DateTime PaidAt { get; set; }
}

/// <summary>
/// Service message about payment refund for suggested post
/// </summary>
public class SuggestedPostRefunded
{
    public string PostId { get; set; } = string.Empty;
    public long MessageId { get; set; }
    public long ChannelId { get; set; }
    public long RefundedTo { get; set; }
    public long Amount { get; set; } // Changed from StarAmount to long
    public DateTime RefundedAt { get; set; }
    public string Reason { get; set; } = string.Empty;
}

/// <summary>
/// Parameters for suggested post
/// </summary>
public class SuggestedPostParameters
{
    public SuggestedPostPrice Price { get; set; } = new();
    public bool AllowPayment { get; set; }
    public TimeSpan? ExpirationTime { get; set; }
    public bool RequireApproval { get; set; } = true;
    public List<string> AllowedContentTypes { get; set; } = new();
    public int MaxTextLength { get; set; } = 4096;
    public bool AllowLinks { get; set; } = true;
    public bool AllowMedia { get; set; } = true;
    public int MaxMediaCount { get; set; } = 10;
    public List<long> RestrictedWords { get; set; } = new();
}

/// <summary>
/// Information about suggested post
/// </summary>
public class SuggestedPostInfo
{
    public string PostId { get; set; } = string.Empty;
    public SuggestedPostPrice Price { get; set; } = new();
    public SuggestedPostStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ExpiresAt { get; set; }
    public string Preview { get; set; } = string.Empty;
    public bool RequiresPayment { get; set; }
    public bool CanBeNegotiated { get; set; }
}

/// <summary>
/// Suggested post creation request
/// </summary>
public class CreateSuggestedPostRequest
{
    public long ChannelId { get; set; }
    public long SuggestedBy { get; set; }
    public string Content { get; set; } = string.Empty;
    public SuggestedPostType Type { get; set; }
    public List<SuggestedPostMediaAttachment> MediaAttachments { get; set; } = new();
    public List<string> Entities { get; set; } = new(); // Changed from MessageEntity to string
    public SuggestedPostPrice Price { get; set; } = new();
    public bool Silent { get; set; }
    public bool DisableWebPagePreview { get; set; }
    public TimeSpan? ExpirationTime { get; set; }
}

/// <summary>
/// Suggested post creation result
/// </summary>
public class CreateSuggestedPostResult
{
    public bool Success { get; set; }
    public SuggestedPost? Post { get; set; }
    public string? ErrorMessage { get; set; }
    public bool RequiresPayment { get; set; }
    public string PaymentUrl { get; set; } = string.Empty;
    public DateTime? ExpiresAt { get; set; }
    public string PostId { get; set; } = string.Empty;
}

/// <summary>
/// Suggested post statistics for channel
/// </summary>
public class ChannelSuggestedPostStatistics
{
    public long ChannelId { get; set; }
    public int TotalSuggestedPosts { get; set; }
    public int PendingPosts { get; set; }
    public int ApprovedPosts { get; set; }
    public int PublishedPosts { get; set; }
    public int RejectedPosts { get; set; }
    public long TotalRevenue { get; set; }
    public double AveragePrice { get; set; }
    public double AcceptanceRate { get; set; }
    public List<UserSuggestedPostStats> UserStats { get; set; } = new();
    public DateTime PeriodStart { get; set; }
    public DateTime PeriodEnd { get; set; }
}

/// <summary>
/// User-specific suggested post statistics
/// </summary>
public class UserSuggestedPostStats
{
    public long UserId { get; set; }
    public int PostsSuggested { get; set; }
    public int PostsPublished { get; set; }
    public int PostsRejected { get; set; }
    public long TotalSpent { get; set; }
    public double SuccessRate { get; set; }
    public DateTime LastActivity { get; set; }
}

/// <summary>
/// Suggested post validation result
/// </summary>
public class SuggestedPostValidationResult
{
    public bool IsValid { get; set; }
    public string? ErrorMessage { get; set; }
    public List<string> ValidationErrors { get; set; } = new();
    public bool RequiresApproval { get; set; }
    public SuggestedPostPrice SuggestedPrice { get; set; } = new();
}
