namespace MyTelegram.Domain.Shared.Messages;

/// <summary>
/// Paid message settings for channels/chats
/// </summary>
public class PaidMessageSettings
{
    public long ChannelId { get; set; }
    public bool Enabled { get; set; }
    public long StarsAmount { get; set; }
    public PaidMessageRestrictionType RestrictionType { get; set; }
    public List<long> ExcludedUsers { get; set; } = new();
    public List<long> IncludedUsers { get; set; } = new();
    public string? WelcomeMessage { get; set; }
    public int MinMessageLength { get; set; }
    public int MaxMessageLength { get; set; }
    public bool AllowMedia { get; set; }
    public bool AllowLinks { get; set; }
    public bool AllowForwards { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public long TotalMessages { get; set; }
    public long TotalStarsEarned { get; set; }
}

public enum PaidMessageRestrictionType
{
    Everyone,
    ContactsOnly,
    PremiumOnly,
    Whitelist,
    Blacklist,
    None
}

/// <summary>
/// Paid message record
/// </summary>
public class PaidMessage
{
    public string Id { get; set; } = string.Empty;
    public long ChannelId { get; set; }
    public long MessageId { get; set; }
    public long SenderId { get; set; }
    public long StarsAmount { get; set; }
    public string TransactionId { get; set; } = string.Empty;
    public DateTime PaidAt { get; set; }
    public bool IsRefunded { get; set; }
    public DateTime? RefundedAt { get; set; }
    public string? RefundReason { get; set; }
    public string MessageContent { get; set; } = string.Empty;
    public PaidMessageType MessageType { get; set; }
    public List<string> MediaIds { get; set; } = new();
    public bool IsVisible { get; set; } // Whether message is visible to other users
    public DateTime? ExpiresAt { get; set; }
}

public enum PaidMessageType
{
    Text,
    Photo,
    Video,
    Audio,
    Document,
    Sticker,
    Location,
    Contact,
    Poll,
    Forward
}

/// <summary>
/// Paid message statistics
/// </summary>
public class PaidMessageStats
{
    public long ChannelId { get; set; }
    public int TotalPaidMessages { get; set; }
    public long TotalStarsEarned { get; set; }
    public int UniqueSenders { get; set; }
    public double AverageStarsPerMessage { get; set; }
    public int MessagesThisWeek { get; set; }
    public int MessagesThisMonth { get; set; }
    public List<DailyStats> DailyBreakdown { get; set; } = new();
    public List<TopSenderStats> TopSenders { get; set; } = new();
    public DateTime PeriodStart { get; set; }
    public DateTime PeriodEnd { get; set; }
}

public class DailyStats
{
    public DateTime Date { get; set; }
    public int MessageCount { get; set; }
    public long StarsEarned { get; set; }
    public int UniqueSenders { get; set; }
}

public class TopSenderStats
{
    public long SenderId { get; set; }
    public string SenderName { get; set; } = string.Empty;
    public int MessageCount { get; set; }
    public long TotalStarsSpent { get; set; }
    public DateTime LastMessageDate { get; set; }
}

/// <summary>
/// Paid message payment request
/// </summary>
public class PaidMessagePaymentRequest
{
    public long UserId { get; set; }
    public long ChannelId { get; set; }
    public string MessageContent { get; set; } = string.Empty;
    public PaidMessageType MessageType { get; set; }
    public List<string> MediaIds { get; set; } = new();
    public bool UseBalanceFirst { get; set; } = true;
    public string? ReplyToMessageId { get; set; }
    public List<long> ReplyMarkupButtons { get; set; } = new();
}

/// <summary>
/// Paid message payment result
/// </summary>
public class PaidMessagePaymentResult
{
    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }
    public string PaidMessageId { get; set; } = string.Empty;
    public long MessageId { get; set; }
    public long StarsSpent { get; set; }
    public string TransactionId { get; set; } = string.Empty;
    public DateTime PaidAt { get; set; }
    public bool RequiresConfirmation { get; set; }
    public string? ConfirmationUrl { get; set; }
}

/// <summary>
/// Channel paid message revenue
/// </summary>
public class PaidMessageRevenue
{
    public long ChannelId { get; set; }
    public long CurrentBalance { get; set; }
    public long AvailableBalance { get; set; }
    public long TotalRevenue { get; set; }
    public int MessagesCount { get; set; }
    public DateTime LastPaymentDate { get; set; }
    public bool WithdrawalEnabled { get; set; }
    public DateTime? NextWithdrawalAt { get; set; }
    public double ConversionRate { get; set; } // Revenue per message
    public List<RevenueBreakdown> RevenueBreakdown { get; set; } = new();
}

public class RevenueBreakdown
{
    public string Period { get; set; } = string.Empty;
    public long Revenue { get; set; }
    public int MessageCount { get; set; }
    public double AvgRevenuePerMessage { get; set; }
    public DateTime Date { get; set; }
}

/// <summary>
/// User paid message history
/// </summary>
public class UserPaidMessageHistory
{
    public long UserId { get; set; }
    public int TotalMessages { get; set; }
    public long TotalStarsSpent { get; set; }
    public int MessagesThisWeek { get; set; }
    public int MessagesThisMonth { get; set; }
    public List<ChannelMessageStats> ChannelStats { get; set; } = new();
    public DateTime FirstMessageDate { get; set; }
    public DateTime LastMessageDate { get; set; }
}

public class ChannelMessageStats
{
    public long ChannelId { get; set; }
    public string ChannelName { get; set; } = string.Empty;
    public int MessageCount { get; set; }
    public long TotalStarsSpent { get; set; }
    public DateTime LastMessageDate { get; set; }
}

/// <summary>
/// Paid message exemption for specific users
/// </summary>
public class PaidMessageExemption
{
    public string Id { get; set; } = string.Empty;
    public long ChannelId { get; set; }
    public long UserId { get; set; }
    public PaidMessageExemptionType Type { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ExpiresAt { get; set; }
    public string? Reason { get; set; }
    public long CreatedBy { get; set; }
}

public enum PaidMessageExemptionType
{
    Permanent,
    Temporary,
    OneTime,
    Conditional
}
