namespace MyTelegram.Domain.Shared.Affiliate;

/// <summary>
/// Star referral program configuration for bots and mini apps
/// </summary>
public class StarReferralProgram
{
    public string Id { get; set; } = string.Empty;
    public long BotId { get; set; }
    public long CreatorId { get; set; }
    public int CommissionPermille { get; set; } // Commission in per mille (‰), e.g. 100 = 10%
    public int DurationMonths { get; set; }
    public DateTime EndDate { get; set; }
    public long DailyRevenuePerUser { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public string? Description { get; set; }
    public string? TermsUrl { get; set; }
    public List<AffiliateTier> Tiers { get; set; } = new();
    public AffiliateSettings Settings { get; set; } = new();
}

/// <summary>
/// Affiliate tier for graduated commission rates
/// </summary>
public class AffiliateTier
{
    public int Tier { get; set; }
    public int MinReferrals { get; set; }
    public int MaxReferrals { get; set; }
    public int CommissionPermille { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
}

/// <summary>
/// Settings for affiliate program behavior
/// </summary>
public class AffiliateSettings
{
    public bool AllowMultipleAccounts { get; set; }
    public bool RequireVerification { get; set; }
    public bool EnableRecurringCommission { get; set; }
    public int MaxCommissionDuration { get; set; } // Days
    public long MinPurchaseAmount { get; set; }
    public List<string> AllowedCountries { get; set; } = new();
    public List<string> ExcludedCountries { get; set; } = new();
    public string? WelcomeMessage { get; set; }
    public bool EnableAffiliateDashboard { get; set; }
}

/// <summary>
/// Affiliate relationship between user and bot/mini app
/// </summary>
public class Affiliate
{
    public string Id { get; set; } = string.Empty;
    public long AffiliateId { get; set; } // The user who is the affiliate
    public long BotId { get; set; }
    public string ReferralCode { get; set; } = string.Empty;
    public string CustomReferralUrl { get; set; } = string.Empty;
    public int CurrentTier { get; set; }
    public DateTime JoinedAt { get; set; }
    public DateTime? LastReferralDate { get; set; }
    public int TotalReferrals { get; set; }
    public int ActiveReferrals { get; set; }
    public long TotalCommission { get; set; }
    public long PendingCommission { get; set; }
    public long WithdrawnCommission { get; set; }
    public bool IsActive { get; set; }
    public bool IsVerified { get; set; }
    public DateTime? VerifiedAt { get; set; }
    public string? Notes { get; set; }
}

/// <summary>
/// Referral record - when someone signs up through an affiliate link
/// </summary>
public class Referral
{
    public string Id { get; set; } = string.Empty;
    public long AffiliateId { get; set; }
    public long ReferredUserId { get; set; }
    public long BotId { get; set; }
    public string ReferralCode { get; set; } = string.Empty;
    public DateTime ReferredAt { get; set; }
    public string? Source { get; set; } // Where the referral came from
    public string? Campaign { get; set; } // Marketing campaign identifier
    public bool IsActive { get; set; }
    public DateTime? FirstPurchaseDate { get; set; }
    public DateTime? LastActiveDate { get; set; }
    public int PurchaseCount { get; set; }
    public long TotalSpent { get; set; }
    public long GeneratedCommission { get; set; }
    public ReferralStatus Status { get; set; }
}

public enum ReferralStatus
{
    Pending,
    Active,
    Inactive,
    Suspended,
    Banned
}

/// <summary>
/// Commission record for affiliate earnings
/// </summary>
public class AffiliateCommission
{
    public string Id { get; set; } = string.Empty;
    public long AffiliateId { get; set; }
    public long ReferralId { get; set; }
    public long BotId { get; set; }
    public long Amount { get; set; }
    public string Currency { get; set; } = "XTR";
    public int CommissionPermille { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? PaidAt { get; set; }
    public string? TransactionId { get; set; }
    public CommissionStatus Status { get; set; }
    public string? PurchaseId { get; set; } // Original purchase that generated commission
    public long OriginalAmount { get; set; }
    public DateTime? EligibleForPayment { get; set; }
    public string? Description { get; set; }
}

public enum CommissionStatus
{
    Pending,
    Approved,
    Paid,
    Rejected,
    Disputed
}

/// <summary>
/// Affiliate statistics for reporting
/// </summary>
public class AffiliateStats
{
    public long AffiliateId { get; set; }
    public long BotId { get; set; }
    public int TotalReferrals { get; set; }
    public int ActiveReferrals { get; set; }
    public int NewReferralsThisMonth { get; set; }
    public int NewReferralsThisWeek { get; set; }
    public long TotalCommission { get; set; }
    public long CommissionThisMonth { get; set; }
    public long CommissionThisWeek { get; set; }
    public long PendingCommission { get; set; }
    public double ConversionRate { get; set; } // Referrals to paying customers
    public double AverageCommissionPerReferral { get; set; }
    public DateTime PeriodStart { get; set; }
    public DateTime PeriodEnd { get; set; }
    public List<DailyAffiliateStats> DailyBreakdown { get; set; } = new();
}

public class DailyAffiliateStats
{
    public DateTime Date { get; set; }
    public int NewReferrals { get; set; }
    public long CommissionEarned { get; set; }
    public int ActiveReferrals { get; set; }
    public long RevenueGenerated { get; set; }
}

/// <summary>
/// Payout request for affiliate commissions
/// </summary>
public class AffiliatePayout
{
    public string Id { get; set; } = string.Empty;
    public long AffiliateId { get; set; }
    public long BotId { get; set; }
    public long Amount { get; set; }
    public string Currency { get; set; } = "XTR";
    public DateTime RequestedAt { get; set; }
    public DateTime? ProcessedAt { get; set; }
    public DateTime? PaidAt { get; set; }
    public PayoutStatus Status { get; set; }
    public string? PaymentMethod { get; set; }
    public string? TransactionHash { get; set; }
    public string? Notes { get; set; }
    public List<string> CommissionIds { get; set; } = new();
    public string? RejectionReason { get; set; }
}

public enum PayoutStatus
{
    Pending,
    Approved,
    Processing,
    Paid,
    Rejected,
    Failed
}

/// <summary>
/// Affiliate link tracking
/// </summary>
public class AffiliateLink
{
    public string Id { get; set; } = string.Empty;
    public long AffiliateId { get; set; }
    public long BotId { get; set; }
    public string Url { get; set; } = string.Empty;
    public string Campaign { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ExpiresAt { get; set; }
    public bool IsActive { get; set; }
    public int ClickCount { get; set; }
    public int ConversionCount { get; set; }
    public double ConversionRate { get; set; }
    public DateTime? LastClickAt { get; set; }
    public DateTime? LastConversionAt { get; set; }
}

/// <summary>
/// Click tracking for affiliate links
/// </summary>
public class AffiliateClick
{
    public string Id { get; set; } = string.Empty;
    public long AffiliateId { get; set; }
    public long BotId { get; set; }
    public string LinkId { get; set; } = string.Empty;
    public string IpAddress { get; set; } = string.Empty;
    public string UserAgent { get; set; } = string.Empty;
    public string? Country { get; set; }
    public string? Referer { get; set; }
    public DateTime ClickedAt { get; set; }
    public bool Converted { get; set; }
    public DateTime? ConvertedAt { get; set; }
    public string? ReferralId { get; set; }
}
