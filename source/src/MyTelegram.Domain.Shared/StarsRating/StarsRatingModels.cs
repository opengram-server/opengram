namespace MyTelegram.Domain.Shared.StarsRating;

/// <summary>
/// User stars rating and level
/// </summary>
public class UserStarsRating
{
    public long UserId { get; set; }
    public long TotalStarsSpent { get; set; }
    public long TotalStarsReceived { get; set; }
    public int StarsLevel { get; set; }
    public string LevelTitle { get; set; } = string.Empty;
    public long StarsToNextLevel { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public List<StarsAchievement> Achievements { get; set; } = new();
    public StarsStatistics Statistics { get; set; } = new();
    public bool IsPremium { get; set; }
    public bool IsVerified { get; set; }
    public double TrustScore { get; set; }
    public string BadgeUrl { get; set; } = string.Empty;
    public List<StarsActivity> RecentActivities { get; set; } = new();
    public RankPosition GlobalRank { get; set; } = new();
    public RankPosition CountryRank { get; set; } = new();
    public RankPosition CityRank { get; set; } = new();
}

/// <summary>
/// Stars level configuration
/// </summary>
public class StarsLevel
{
    public int Level { get; set; }
    public string Title { get; set; } = string.Empty;
    public long RequiredStars { get; set; }
    public string Color { get; set; } = string.Empty;
    public string Icon { get; set; } = string.Empty;
    public List<string> Benefits { get; set; } = new();
    public bool IsSpecial { get; set; }
    public string Badge { get; set; } = string.Empty;
    public double TrustMultiplier { get; set; }
}

/// <summary>
/// Achievement earned through stars
/// </summary>
public class StarsAchievement
{
    public string Id { get; set; } = string.Empty;
    public long UserId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Icon { get; set; } = string.Empty;
    public DateTime EarnedAt { get; set; }
    public long RequiredStars { get; set; }
    public int Points { get; set; }
    public bool IsPublic { get; set; }
    public bool IsRare { get; set; }
    public string Category { get; set; } = string.Empty;
    public int Progress { get; set; }
    public int MaxProgress { get; set; }
}

/// <summary>
/// Statistics for user stars activity
/// </summary>
public class StarsStatistics
{
    public long UserId { get; set; }
    public int MessagesSentWithStars { get; set; }
    public int GiftsSent { get; set; }
    public int PostsSuggested { get; set; }
    public int PaymentsMade { get; set; }
    public double AverageSpendPerMonth { get; set; }
    public DateTime LastActivity { get; set; }
    public int ActiveDaysCount { get; set; }
    public List<MonthlyStarsStats> MonthlyStats { get; set; } = new();
    public List<DailyStarsStats> DailyStats { get; set; } = new();
    public double EngagementRate { get; set; }
    public long TotalTransactions { get; set; }
}

/// <summary>
/// Monthly stars statistics
/// </summary>
public class MonthlyStarsStats
{
    public int Year { get; set; }
    public int Month { get; set; }
    public long StarsSpent { get; set; }
    public long StarsReceived { get; set; }
    public int Transactions { get; set; }
    public double EngagementScore { get; set; }
}

/// <summary>
/// Daily stars statistics
/// </summary>
public class DailyStarsStats
{
    public DateTime Date { get; set; }
    public long StarsSpent { get; set; }
    public long StarsReceived { get; set; }
    public int Transactions { get; set; }
    public int MessagesWithStars { get; set; }
    public int GiftsSent { get; set; }
}

/// <summary>
/// Activity record for stars
/// </summary>
public class StarsActivity
{
    public string Id { get; set; } = string.Empty;
    public long UserId { get; set; }
    public StarsActivityType Type { get; set; }
    public long Amount { get; set; }
    public DateTime ActivityDate { get; set; }
    public string Description { get; set; } = string.Empty;
    public long? RelatedUserId { get; set; }
    public string? RelatedChannelId { get; set; }
    public string? RelatedPostId { get; set; }
    public bool IsPublic { get; set; }
    public string Emoji { get; set; } = string.Empty;
    public int PointsAwarded { get; set; }
}

/// <summary>
/// Type of stars activity
/// </summary>
public enum StarsActivityType
{
    GiftSent,
    GiftReceived,
    MessageSent,
    PostSuggested,
    PostPublished,
    PaymentMade,
    PaymentReceived,
    BoostReceived,
    SubscriptionPaid,
    ManualUpdate,
    DonationMade,
    AchievementUnlocked,
    LevelUp
}

/// <summary>
/// Rank position in leaderboard
/// </summary>
public class RankPosition
{
    public int Position { get; set; }
    public long UserId { get; set; }
    public long TotalStars { get; set; }
    public int Level { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string Avatar { get; set; } = string.Empty;
    public DateTime LastUpdated { get; set; }
}

/// <summary>
/// Stars leaderboard
/// </summary>
public class StarsLeaderboard
{
    public string Id { get; set; } = string.Empty;
    public LeaderboardType Type { get; set; }
    public LeaderboardPeriod Period { get; set; }
    public DateTime GeneratedAt { get; set; }
    public DateTime PeriodStart { get; set; }
    public DateTime PeriodEnd { get; set; }
    public List<RankPosition> Rankings { get; set; } = new();
    public int TotalParticipants { get; set; }
    public long MinStarsRequired { get; set; }
    public bool IsActive { get; set; }
}

/// <summary>
/// Type of leaderboard
/// </summary>
public enum LeaderboardType
{
    Global,
    Country,
    City,
    Channel,
    AgeGroup,
    Interest
}

/// <summary>
/// Period for leaderboard
/// </summary>
public enum LeaderboardPeriod
{
    Daily,
    Weekly,
    Monthly,
    Yearly,
    AllTime
}

/// <summary>
/// Stars rating update request
/// </summary>
public class UpdateStarsRatingRequest
{
    public long UserId { get; set; }
    public long StarsAmount { get; set; }
    public StarsActivityType ActivityType { get; set; }
    public string Description { get; set; } = string.Empty;
    public long? RelatedUserId { get; set; }
    public string? RelatedChannelId { get; set; }
    public bool IsPublic { get; set; }
    public string? Comment { get; set; }
}

/// <summary>
/// Stars rating calculation result
/// </summary>
public class StarsRatingResult
{
    public long UserId { get; set; }
    public int NewLevel { get; set; }
    public string LevelTitle { get; set; } = string.Empty;
    public long TotalStars { get; set; }
    public bool LeveledUp { get; set; }
    public List<StarsAchievement> NewAchievements { get; set; } = new();
    public double TrustScore { get; set; }
    public RankPosition NewRank { get; set; } = new();
}

/// <summary>
/// Stars level configuration request
/// </summary>
public class ConfigureStarsLevelRequest
{
    public int Level { get; set; }
    public string Title { get; set; } = string.Empty;
    public long RequiredStars { get; set; }
    public string Color { get; set; } = string.Empty;
    public string Icon { get; set; } = string.Empty;
    public List<string> Benefits { get; set; } = new();
    public bool IsSpecial { get; set; }
    public string Badge { get; set; } = string.Empty;
    public double TrustMultiplier { get; set; }
}

/// <summary>
/// Achievement configuration
/// </summary>
public class StarsAchievementConfig
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Icon { get; set; } = string.Empty;
    public long RequiredStars { get; set; }
    public int Points { get; set; }
    public bool IsPublic { get; set; }
    public bool IsRare { get; set; }
    public string Category { get; set; } = string.Empty;
    public StarsActivityType ActivityType { get; set; }
    public int RequiredCount { get; set; }
    public string Condition { get; set; } = string.Empty;
}

/// <summary>
/// User trust score calculation
/// </summary>
public class TrustScoreCalculation
{
    public long UserId { get; set; }
    public double BaseScore { get; set; }
    public double ActivityBonus { get; set; }
    public double ConsistencyBonus { get; set; }
    public double SocialBonus { get; set; }
    public double VerificationBonus { get; set; }
    public double PremiumBonus { get; set; }
    public double FinalScore { get; set; }
    public DateTime CalculatedAt { get; set; }
    public List<string> Factors { get; set; } = new();
}
