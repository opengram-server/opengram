namespace MyTelegram.Domain.Shared.Media;

/// <summary>
/// Paid media model - media that requires stars to view
/// </summary>
public class PaidMedia
{
    public string Id { get; set; } = string.Empty;
    public long ChannelId { get; set; }
    public long MessageId { get; set; }
    public PaidMediaType Type { get; set; }
    public long StarsAmount { get; set; }
    public bool IsExtended { get; set; }
    public bool IsUnlocked { get; set; }
    public DateTime? UnlockedAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<PaidMediaItem> MediaItems { get; set; } = new();
    public string? Title { get; set; }
    public string? Description { get; set; }
    public string? Thumbnail { get; set; }
    public int TotalViews { get; set; }
    public int UniqueUnlocks { get; set; }
    public bool IsActive { get; set; }
    public DateTime? ExpiresAt { get; set; }
}

public enum PaidMediaType
{
    Photo,
    Video,
    Audio,
    Document,
    Album
}

/// <summary>
/// Individual media item within paid media
/// </summary>
public class PaidMediaItem
{
    public string Id { get; set; } = string.Empty;
    public string FileId { get; set; } = string.Empty;
    public string FileName { get; set; } = string.Empty;
    public string MimeType { get; set; } = string.Empty;
    public long FileSize { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }
    public int Duration { get; set; } // For video/audio
    public string? Thumbnail { get; set; }
    public string? Caption { get; set; }
    public DateTime AddedAt { get; set; }
}

/// <summary>
/// Paid media purchase/record
/// </summary>
public class PaidMediaPurchase
{
    public string Id { get; set; } = string.Empty;
    public long UserId { get; set; }
    public long ChannelId { get; set; }
    public long MessageId { get; set; }
    public long StarsAmount { get; set; }
    public DateTime PurchasedAt { get; set; }
    public string TransactionId { get; set; } = string.Empty;
    public bool IsRefunded { get; set; }
    public DateTime? RefundedAt { get; set; }
    public string? RefundReason { get; set; }
}

/// <summary>
/// Paid media statistics for channel owners
/// </summary>
public class PaidMediaStats
{
    public long ChannelId { get; set; }
    public int TotalPaidMedia { get; set; }
    public int ActivePaidMedia { get; set; }
    public int TotalPurchases { get; set; }
    public long TotalStarsEarned { get; set; }
    public long TotalViews { get; set; }
    public double ConversionRate { get; set; } // Purchases / Views
    public DateTime PeriodStart { get; set; }
    public DateTime PeriodEnd { get; set; }
    public List<PaidMediaItemStats> ItemStats { get; set; } = new();
}

/// <summary>
/// Statistics for individual paid media item
/// </summary>
public class PaidMediaItemStats
{
    public string PaidMediaId { get; set; } = string.Empty;
    public int Views { get; set; }
    public int Purchases { get; set; }
    public long StarsEarned { get; set; }
    public double ConversionRate { get; set; }
    public DateTime FirstPurchase { get; set; }
    public DateTime LastPurchase { get; set; }
}

/// <summary>
/// Paid media pricing settings
/// </summary>
public class PaidMediaPricing
{
    public long ChannelId { get; set; }
    public bool Enabled { get; set; }
    public long DefaultStarsAmount { get; set; }
    public long MinStarsAmount { get; set; }
    public long MaxStarsAmount { get; set; }
    public List<PaidMediaPriceRule> Rules { get; set; } = new();
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

/// <summary>
/// Pricing rule for different media types
/// </summary>
public class PaidMediaPriceRule
{
    public PaidMediaType MediaType { get; set; }
    public long StarsAmount { get; set; }
    public bool Enabled { get; set; }
    public int MinFileSize { get; set; } // In bytes
    public int MaxFileSize { get; set; } // In bytes
}

/// <summary>
/// Paid media unlock request
/// </summary>
public class PaidMediaUnlockRequest
{
    public long UserId { get; set; }
    public long ChannelId { get; set; }
    public long MessageId { get; set; }
    public string PaidMediaId { get; set; } = string.Empty;
    public bool UseBalanceFirst { get; set; } = true;
    public string? PaymentSource { get; set; }
}

/// <summary>
/// Paid media unlock result
/// </summary>
public class PaidMediaUnlockResult
{
    public bool Success { get; set; }
    public string PaidMediaId { get; set; } = string.Empty;
    public List<PaidMediaItem> UnlockedMedia { get; set; } = new();
    public long StarsSpent { get; set; }
    public string TransactionId { get; set; } = string.Empty;
    public DateTime UnlockedAt { get; set; }
    public string? ErrorMessage { get; set; }
    public bool RefundRequired { get; set; }
}
