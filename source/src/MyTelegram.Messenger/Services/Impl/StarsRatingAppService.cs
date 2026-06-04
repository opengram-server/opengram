using Microsoft.Extensions.Logging;
using MyTelegram.Schema;
using MyTelegram.Messenger.Services;
using MyTelegram.Messenger.Services.Interfaces;
using MyTelegram.Domain.Shared;
using MyTelegram.Domain.Shared.StarsRating;
using EventFlow.Commands;
using EventFlow.Aggregates.ExecutionResults;

namespace MyTelegram.Messenger.Services.Impl;

/// <summary>
/// Интерфейс сервиса рейтинга по звёздам
/// </summary>
public interface IStarsRatingAppService
{
    Task<StarsRatingResult> UpdateRatingAsync(UpdateStarsRatingRequest request);
    Task<StarsRating?> GetUserRatingAsync(long userId);
    Task<StarsLeaderboard> GetLeaderboardAsync(LeaderboardType type, LeaderboardPeriod period, int offset = 0, int limit = 50);
    Task<List<StarsRating>> GetTopUsersAsync(LeaderboardType type, int count = 100);
    Task<bool> ConfigureLevelAsync(int level, ConfigureStarsLevelRequest config);
    Task<bool> AddAchievementAsync(StarsAchievementConfig config);
    Task<List<StarsAchievement>> GetUserAchievementsAsync(long userId);
    Task<double> CalculateTrustScoreAsync(long userId);
    Task<bool> UpdateLeaderboardAsync(MyTelegram.Domain.Shared.StarsRating.LeaderboardType type, MyTelegram.Domain.Shared.StarsRating.LeaderboardPeriod period);
    Task<StarsStatistics> GetUserStatisticsAsync(long userId, DateTime? from = null, DateTime? to = null);
    Task<List<StarsActivity>> GetUserActivityAsync(long userId, int offset = 0, int limit = 50);
}

/// <summary>
/// Сервис управления рейтингом по звёздам
/// </summary>
public sealed class StarsRatingAppService(
    ILogger<StarsRatingAppService> logger,
    IQueryProcessor queryProcessor,
    ICommandBus commandBus,
    IUserAppService userAppService,
    IStarsAppService starsAppService) : IStarsRatingAppService
{
    public async Task<StarsRatingResult> UpdateRatingAsync(UpdateStarsRatingRequest request)
    {
        logger.LogInformation("Updating stars rating for user {UserId} with {Amount} stars for activity {ActivityType}", 
            request.UserId, request.StarsAmount, request.ActivityType);

        var userRating = await GetUserRatingAsync(request.UserId);
        if (userRating == null)
        {
            userRating = await CreateInitialRatingAsync(request.UserId);
        }

        var oldLevel = userRating.StarsLevel;
        var oldTotalStars = userRating.TotalStarsSpent;

        // Обновляем общее количество звёзд
        userRating.TotalStarsSpent += request.StarsAmount;
        // userRating.UpdatedAt = DateTime.UtcNow;

        // Вычисляем новый уровень
        var newLevel = await CalculateUserLevelAsync(userRating.TotalStarsSpent);
        userRating.StarsLevel = newLevel.Level;
        userRating.LevelTitle = newLevel.Title;
        // userRating.StarsToNextLevel = CalculateStarsToNextLevel(userRating.TotalStarsSpent);

        // Вычисляем показатель доверия
        var trustScore = await CalculateTrustScoreAsync(request.UserId);
        userRating.TrustScore = trustScore;

        // Записываем активность
        var activity = new MyTelegram.Domain.Shared.StarsRating.StarsActivity
        {
            Id = Guid.NewGuid().ToString(),
            UserId = request.UserId,
            Type = (MyTelegram.Domain.Shared.StarsRating.StarsActivityType)request.ActivityType,
            Amount = request.StarsAmount,
            ActivityDate = DateTime.UtcNow,
            Description = request.Description,
            RelatedUserId = request.RelatedUserId,
            RelatedChannelId = request.RelatedChannelId
        };

        // Проверяем достижения
        var userStarsRating = new UserStarsRating
        {
            UserId = userRating.UserId,
            TotalStarsSpent = userRating.TotalStarsSpent,
            TotalStarsReceived = userRating.TotalStarsReceived,
            StarsLevel = userRating.StarsLevel,
            LevelTitle = userRating.LevelTitle,
            TrustScore = userRating.TrustScore,
            IsPremium = userRating.IsPremium,
            IsVerified = userRating.IsVerified,
            StarsToNextLevel = userRating.StarsToNextLevel,
            UpdatedAt = DateTime.UtcNow,
            CreatedAt = userRating.LastUpdated
        };
        var newAchievements = await CheckAchievementsAsync(userStarsRating, activity);

        // Обновляем позиции в рейтинге
        // userRating.GlobalRank = await CalculateGlobalRankAsync(userRating.UserId);
        var globalRank = await GetUserGlobalRankAsync(request.UserId);
        userRating.GlobalRank = globalRank;

        var userId = UserId.With(request.UserId.ToString());
        var command = new UpdateStarsRatingCommand(userId)
        {
            UserId = request.UserId,
            TotalStarsSpent = userRating.TotalStarsSpent,
            TotalStarsReceived = userRating.TotalStarsReceived,
            StarsLevel = userRating.StarsLevel,
            LevelTitle = userRating.LevelTitle,
            StarsToNextLevel = userRating.StarsToNextLevel,
            UpdatedAt = DateTime.UtcNow,
            TrustScore = trustScore
        };

        await commandBus.PublishAsync<UserAggregate, UserId, IExecutionResult>(command, CancellationToken.None);

        // Записываем активность
        await RecordActivityAsync(activity);

        // Выдаём достижения
        foreach (var achievement in newAchievements)
        {
            await AwardAchievementAsync(achievement, request.UserId);
        }

        var result = new StarsRatingResult
        {
            UserId = request.UserId,
            NewLevel = userRating.StarsLevel,
            LevelTitle = userRating.LevelTitle,
            TotalStars = userRating.TotalStarsSpent,
            LeveledUp = newLevel.Level > oldLevel,
            NewAchievements = newAchievements,
            TrustScore = trustScore,
            NewRank = globalRank
        };

        logger.LogInformation("Stars rating updated for user {UserId}: Level {Level}, Total stars {TotalStars}", 
            request.UserId, result.NewLevel, result.TotalStars);

        return result;
    }

    public async Task<StarsRating?> GetUserRatingAsync(long userId)
    {
        var domainRating = await queryProcessor.ProcessAsync(new GetUserStarsRatingQuery(userId));
        
        if (domainRating == null)
        {
            return await CreateInitialRatingAsync(userId);
        }

        // Преобразуем доменный тип в локальный
        var rating = new StarsRating
        {
            UserId = domainRating.UserId,
            TotalStarsSpent = domainRating.TotalStarsSpent,
            TotalStarsReceived = domainRating.TotalStarsReceived,
            StarsLevel = domainRating.StarsLevel,
            LevelTitle = domainRating.LevelTitle,
            TrustScore = domainRating.TrustScore,
            LastUpdated = DateTime.UtcNow
        };

        return rating;
    }

    public async Task<StarsLeaderboard> GetLeaderboardAsync(LeaderboardType type, LeaderboardPeriod period, int offset = 0, int limit = 50)
    {
        logger.LogInformation("Getting leaderboard for type {Type} and period {Period}", type, period);

        var query = new GetStarsLeaderboardQuery(type, period, limit);
        var entries = await queryProcessor.ProcessAsync(query);

        if (entries == null || !entries.Any())
        {
            // Если рейтинг ещё не сформирован, генерируем его
            await UpdateLeaderboardAsync(MapLeaderboardType(type), MapLeaderboardPeriod(period));
            entries = await queryProcessor.ProcessAsync(query);
        }

        return new StarsLeaderboard
        {
            Id = Guid.NewGuid().ToString(),
            Type = MapLeaderboardType(type),
            Period = MapLeaderboardPeriod(period),
            GeneratedAt = DateTime.UtcNow,
            Rankings = entries?.Select(e => new RankPosition 
            { 
                UserId = e.UserId,
                Position = e.Position,
                TotalStars = e.TotalStars
            }).ToList() ?? new List<RankPosition>()
        };
    }

    public async Task<List<StarsRating>> GetTopUsersAsync(LeaderboardType type, int count = 100)
    {
        var query = new GetTopUsersQuery(type, LeaderboardPeriod.AllTime, count);
        var users = await queryProcessor.ProcessAsync(query);
        
        return users?.Select(u => new StarsRating
        {
            UserId = u.UserId,
            TotalStarsSpent = 0, // Значение по умолчанию
            TotalStarsReceived = 0, // Значение по умолчанию
            StarsLevel = 1, // Уровень по умолчанию
            LevelTitle = "Newbie", // Название по умолчанию
            TrustScore = 100 // Показатель по умолчанию
        }).ToList() ?? new List<StarsRating>();
    }

    public async Task<bool> ConfigureLevelAsync(int level, ConfigureStarsLevelRequest config)
    {
        logger.LogInformation("Configuring stars level {Level}", level);

        if (!await IsAdminAsync())
        {
            throw new UnauthorizedAccessException("Admin access required");
        }

        // Системная команда - используем системный userId (0)
        var systemUserId = UserId.With("0");
        var command = new ConfigureStarsLevelCommand(systemUserId)
        {
            Level = level,
            Title = config.Title,
            RequiredStars = config.RequiredStars,
            Color = config.Color,
            Icon = config.Icon,
            Benefits = config.Benefits,
            IsSpecial = config.IsSpecial,
            Badge = config.Badge,
            TrustMultiplier = config.TrustMultiplier,
            ConfiguredAt = DateTime.UtcNow
        };

        await commandBus.PublishAsync<UserAggregate, UserId, IExecutionResult>(command, CancellationToken.None);

        logger.LogInformation("Stars level {Level} configured successfully", level);
        return true;
    }

    public async Task<bool> AddAchievementAsync(StarsAchievementConfig config)
    {
        logger.LogInformation("Adding achievement {Name}", config.Name);

        if (!await IsAdminAsync())
        {
            throw new UnauthorizedAccessException("Admin access required");
        }

        // Системная команда - используем системный userId (0)
        var systemUserId = UserId.With("0");
        var command = new AddStarsAchievementCommand(systemUserId)
        {
            AchievementId = config.Id,
            Name = config.Name,
            Description = config.Description,
            Icon = config.Icon,
            RequiredStars = config.RequiredStars,
            Points = config.Points,
            IsPublic = config.IsPublic,
            IsRare = config.IsRare,
            Category = config.Category,
            ActivityType = config.ActivityType,
            RequiredCount = config.RequiredCount,
            Condition = config.Condition,
            AddedAt = DateTime.UtcNow
        };

        await commandBus.PublishAsync<UserAggregate, UserId, IExecutionResult>(command, CancellationToken.None);

        logger.LogInformation("Achievement {Name} added successfully", config.Name);
        return true;
    }

    public async Task<List<StarsAchievement>> GetUserAchievementsAsync(long userId)
    {
        var achievements = await queryProcessor.ProcessAsync(new GetUserStarsAchievementsQuery(userId));
        
        return achievements?.ToList() ?? new List<StarsAchievement>();
    }

    public async Task<double> CalculateTrustScoreAsync(long userId)
    {
        logger.LogInformation("Calculating trust score for user {UserId}", userId);

        var userRating = await GetUserRatingAsync(userId);
        if (userRating == null)
        {
            return 0.0;
        }

        var calculation = new TrustScoreCalculation
        {
            UserId = userId,
            BaseScore = Math.Min(userRating.TotalStarsSpent / 10000.0, 50.0), // Базовый показатель не больше 50
            ActivityBonus = await CalculateActivityBonus(userId),
            ConsistencyBonus = await CalculateConsistencyBonus(userId),
            SocialBonus = await CalculateSocialBonus(userId),
            VerificationBonus = userRating.IsVerified ? 10.0 : 0.0,
            PremiumBonus = userRating.IsPremium ? 5.0 : 0.0,
            CalculatedAt = DateTime.UtcNow
        };

        calculation.FinalScore = calculation.BaseScore + 
                               calculation.ActivityBonus + 
                               calculation.ConsistencyBonus + 
                               calculation.SocialBonus + 
                               calculation.VerificationBonus + 
                               calculation.PremiumBonus;

        // Удерживаем итоговый показатель в допустимых границах
        calculation.FinalScore = Math.Min(Math.Max(calculation.FinalScore, 0.0), 100.0);

        var userIdObj = UserId.With(userId.ToString());
        var command = new UpdateTrustScoreCommand(userIdObj)
        {
            UserId = userId,
            TrustScore = calculation.FinalScore,
            CalculatedAt = DateTime.UtcNow,
            Factors = new List<string>
            {
                $"Base: {calculation.BaseScore:F2}",
                $"Activity: {calculation.ActivityBonus:F2}",
                $"Consistency: {calculation.ConsistencyBonus:F2}",
                $"Social: {calculation.SocialBonus:F2}",
                $"Verification: {calculation.VerificationBonus:F2}",
                $"Premium: {calculation.PremiumBonus:F2}"
            }
        };

        await commandBus.PublishAsync<UserAggregate, UserId, IExecutionResult>(command, CancellationToken.None);

        logger.LogInformation("Trust score calculated for user {UserId}: {Score:F2}", userId, calculation.FinalScore);
        return calculation.FinalScore;
    }

    public async Task<bool> UpdateLeaderboardAsync(MyTelegram.Domain.Shared.StarsRating.LeaderboardType type, MyTelegram.Domain.Shared.StarsRating.LeaderboardPeriod period)
    {
        logger.LogInformation("Updating leaderboard for type {Type} and period {Period}", type, period);

        // Получаем всех пользователей с активностью по звёздам
        var users = await GetAllActiveUsersAsync();
        var rankings = new List<RankPosition>();

        // Считаем рейтинг за выбранный период
        var (periodStart, periodEnd) = GetPeriodDates(period);

        foreach (var user in users)
        {
            var starsInPeriod = await GetStarsInPeriodAsync(user.UserId, periodStart, periodEnd);
            
            if (starsInPeriod > 0)
            {
                var rank = new RankPosition
                {
                    UserId = user.UserId,
                    TotalStars = starsInPeriod,
                    Level = user.StarsLevel,
                    UserName = await GetUserNameAsync(user.UserId),
                    LastUpdated = DateTime.UtcNow
                };

                rankings.Add(rank);
            }
        }

        // Сортируем по убыванию общего количества звёзд
        rankings = rankings.OrderByDescending(r => r.TotalStars)
                          .Select((r, index) => { r.Position = index + 1; return r; })
                          .ToList();

        var leaderboard = new StarsLeaderboard
        {
            Id = Guid.NewGuid().ToString(),
            Type = (MyTelegram.Domain.Shared.StarsRating.LeaderboardType)type,
            Period = (MyTelegram.Domain.Shared.StarsRating.LeaderboardPeriod)period,
            GeneratedAt = DateTime.UtcNow,
            PeriodStart = periodStart,
            PeriodEnd = periodEnd,
            Rankings = rankings,
            TotalParticipants = rankings.Count,
            MinStarsRequired = rankings.Any() ? rankings.LastOrDefault()?.TotalStars ?? 0 : 0,
            IsActive = true
        };

        // Системная команда - используем системный userId (0)
        var systemUserId = UserId.With("0");
        var command = new UpdateStarsLeaderboardCommand(systemUserId)
        {
            Leaderboard = leaderboard,
            UpdatedAt = DateTime.UtcNow
        };

        await commandBus.PublishAsync<UserAggregate, UserId, IExecutionResult>(command, CancellationToken.None);

        logger.LogInformation("Leaderboard updated successfully for {Type} - {Period} with {Count} participants", 
            type, period, rankings.Count);

        return true;
    }

    public async Task<StarsStatistics> GetUserStatisticsAsync(long userId, DateTime? from = null, DateTime? to = null)
    {
        logger.LogInformation("Getting stars statistics for user {UserId}", userId);

        var query = new GetUserStarsStatisticsQuery(userId, from ?? DateTime.UtcNow.AddDays(-30), to ?? DateTime.UtcNow);

        var stats = await queryProcessor.ProcessAsync(query);
        
        return stats ?? new StarsStatistics
        {
            UserId = userId,
            TotalStarsSpent = 0,
            TotalStarsReceived = 0,
            AverageTransactionSize = 0,
            GiftsSent = 0,
            GiftsReceived = 0,
            From = DateTime.UtcNow.AddDays(-30),
            To = DateTime.UtcNow
        };
    }

    public async Task<List<StarsActivity>> GetUserActivityAsync(long userId, int offset = 0, int limit = 50)
    {
        var query = new GetUserStarsActivityQuery(userId, DateTime.UtcNow.AddDays(-30), DateTime.UtcNow, limit);
        var activities = await queryProcessor.ProcessAsync(query);
        
        return activities?.ToList() ?? new List<StarsActivity>();
    }

    private async Task<StarsRating> CreateInitialRatingAsync(long userId)
    {
        var user = await userAppService.GetAsync(userId);
        
        var userRating = new UserStarsRating
        {
            UserId = userId,
            TotalStarsSpent = 0,
            TotalStarsReceived = 0,
            StarsLevel = 1,
            LevelTitle = "New Star",
            TrustScore = 100.0,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            IsPremium = user?.Premium ?? false,
            IsVerified = false,
            StarsToNextLevel = 100
        };

        var userIdObj = UserId.With(userId.ToString());
        var command = new CreateStarsRatingCommand(userIdObj)
        {
            UserId = userId,
            Rating = userRating,
            CreatedAt = DateTime.UtcNow
        };

        await commandBus.PublishAsync<UserAggregate, UserId, IExecutionResult>(command, CancellationToken.None);

        // Преобразуем в StarsRating для возврата
        var rating = new StarsRating
        {
            UserId = userId,
            TotalStarsSpent = 0,
            TotalStarsReceived = 0,
            StarsLevel = 1,
            LevelTitle = "New Star",
            TrustScore = 100.0,
            LastUpdated = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            IsPremium = user?.Premium ?? false,
            IsVerified = false,
            StarsToNextLevel = 100
        };

        return rating;
    }

    private async Task<StarsLevel> CalculateUserLevelAsync(long totalStars)
    {
        // Получаем все уровни и выбираем подходящий
        var levels = await GetAllLevelsAsync();
        var currentLevel = levels.LastOrDefault(l => totalStars >= l.RequiredStars) ?? levels.First();
        
        return currentLevel;
    }

    private async Task<List<MyTelegram.Domain.Shared.StarsRating.StarsAchievement>> CheckAchievementsAsync(UserStarsRating rating, MyTelegram.Domain.Shared.StarsRating.StarsActivity activity)
    {
        var achievements = new List<MyTelegram.Domain.Shared.StarsRating.StarsAchievement>();
        var achievementConfigs = await GetAllAchievementConfigsAsync();

        foreach (var config in achievementConfigs)
        {
            if (await IsAchievementEarnedAsync(rating.UserId, config.Id))
            {
                continue; // Уже получено
            }

            if (await CheckAchievementConditionAsync(rating, activity, config))
            {
                var achievement = new MyTelegram.Domain.Shared.StarsRating.StarsAchievement
                {
                    Id = config.Id,
                    UserId = rating.UserId,
                    Name = config.Name,
                    Description = config.Description,
                    Icon = config.Icon,
                    EarnedAt = DateTime.UtcNow,
                    RequiredStars = config.RequiredStars,
                    Points = config.Points,
                    IsPublic = config.IsPublic,
                    IsRare = config.IsRare,
                    Category = config.Category,
                    Progress = 1,
                    MaxProgress = 1
                };

                achievements.Add(achievement);
            }
        }

        return achievements;
    }

    private async Task<bool> CheckAchievementConditionAsync(UserStarsRating rating, MyTelegram.Domain.Shared.StarsRating.StarsActivity activity, StarsAchievementConfig config)
    {
        return config.ActivityType switch
        {
            MyTelegram.Domain.Shared.StarsRating.StarsActivityType.GiftSent => config.RequiredCount <= rating.Statistics.GiftsSent,
            MyTelegram.Domain.Shared.StarsRating.StarsActivityType.MessageSent => config.RequiredCount <= rating.Statistics.MessagesSentWithStars,
            MyTelegram.Domain.Shared.StarsRating.StarsActivityType.PostSuggested => config.RequiredCount <= rating.Statistics.PostsSuggested,
            _ => rating.TotalStarsSpent >= config.RequiredStars
        };
    }

    private async Task<bool> IsAchievementEarnedAsync(long userId, string achievementId)
    {
        var earnedAchievements = await GetUserAchievementsAsync(userId);
        return earnedAchievements.Any(a => a.Id == achievementId);
    }

    private async Task AwardAchievementAsync(MyTelegram.Domain.Shared.StarsRating.StarsAchievement achievement, long userId)
    {
        var userIdObj = UserId.With(userId.ToString());
        var command = new AwardStarsAchievementCommand(userIdObj)
        {
            UserId = userId,
            Achievement = achievement,
            AwardedAt = DateTime.UtcNow
        };

        await commandBus.PublishAsync<UserAggregate, UserId, IExecutionResult>(command, CancellationToken.None);
    }

    private async Task<RankPosition> GetUserGlobalRankAsync(long userId)
    {
        var rankings = await GetTopUsersAsync(LeaderboardType.Global, 10000);
        var userRank = rankings.FirstOrDefault(u => u.UserId == userId);
        
        if (userRank == null)
        {
            return new RankPosition { UserId = userId, Position = rankings.Count + 1 };
        }

        return new RankPosition
        {
            UserId = userId,
            Position = rankings.IndexOf(userRank) + 1,
            TotalStars = userRank.TotalStarsSpent,
            Level = userRank.StarsLevel,
            UserName = await GetUserNameAsync(userId),
            LastUpdated = DateTime.UtcNow
        };
    }

    private async Task RecordActivityAsync(MyTelegram.Domain.Shared.StarsRating.StarsActivity activity)
    {
        var userIdObj = UserId.With(activity.UserId.ToString());
        var command = new RecordStarsActivityCommand(userIdObj)
        {
            Activity = activity,
            RecordedAt = DateTime.UtcNow
        };

        await commandBus.PublishAsync<UserAggregate, UserId, IExecutionResult>(command, CancellationToken.None);
    }

    private static string GetActivityEmoji(MyTelegram.Domain.Shared.StarsRating.StarsActivityType type)
    {
        return type switch
        {
            MyTelegram.Domain.Shared.StarsRating.StarsActivityType.GiftSent => "🎁",
            MyTelegram.Domain.Shared.StarsRating.StarsActivityType.GiftReceived => "🎁",
            MyTelegram.Domain.Shared.StarsRating.StarsActivityType.MessageSent => "💬",
            MyTelegram.Domain.Shared.StarsRating.StarsActivityType.PostSuggested => "📝",
            MyTelegram.Domain.Shared.StarsRating.StarsActivityType.PostPublished => "✅",
            MyTelegram.Domain.Shared.StarsRating.StarsActivityType.PaymentMade => "💸",
            MyTelegram.Domain.Shared.StarsRating.StarsActivityType.PaymentReceived => "💰",
            MyTelegram.Domain.Shared.StarsRating.StarsActivityType.BoostReceived => "🚀",
            MyTelegram.Domain.Shared.StarsRating.StarsActivityType.SubscriptionPaid => "💎",
            MyTelegram.Domain.Shared.StarsRating.StarsActivityType.DonationMade => "❤️",
            MyTelegram.Domain.Shared.StarsRating.StarsActivityType.AchievementUnlocked => "🏆",
            MyTelegram.Domain.Shared.StarsRating.StarsActivityType.LevelUp => "⭐",
            _ => "✨"
        };
    }

    private async Task<double> CalculateActivityBonus(long userId)
    {
        var stats = await GetUserStatisticsAsync(userId, DateTime.UtcNow.AddDays(-30), DateTime.UtcNow);
        var activityScore = Math.Min(stats.TotalTransactions / 10.0, 15.0);
        return activityScore;
    }

    private async Task<double> CalculateConsistencyBonus(long userId)
    {
        var stats = await GetUserStatisticsAsync(userId, DateTime.UtcNow.AddDays(-30), DateTime.UtcNow);
        var consistencyScore = Math.Min(stats.ActiveDaysCount / 30.0 * 10.0, 10.0);
        return consistencyScore;
    }

    private async Task<double> CalculateSocialBonus(long userId)
    {
        // Считаем социальный бонус по подаркам, предложенным постам и т.п.
        var stats = await GetUserStatisticsAsync(userId, DateTime.UtcNow.AddDays(-30), DateTime.UtcNow);
        var socialScore = Math.Min((stats.GiftsSent + stats.PostsSuggested) / 5.0, 10.0);
        return socialScore;
    }

    private async Task<long> GetLevelRequirementAsync(int level)
    {
        var levels = await GetAllLevelsAsync();
        return levels.FirstOrDefault(l => l.Level == level)?.RequiredStars ?? 0;
    }

    private async Task<List<StarsLevel>> GetAllLevelsAsync()
    {
        // В реальной реализации эти данные брались бы из конфигурации или базы
        return await Task.FromResult(new List<StarsLevel>
        {
            new() { Level = 1, Title = "New Star", RequiredStars = 0, Color = "#808080", TrustMultiplier = 1.0M },
            new() { Level = 2, Title = "Rising Star", RequiredStars = 1000, Color = "#C0C0C0", TrustMultiplier = 1.1M },
            new() { Level = 3, Title = "Shining Star", RequiredStars = 5000, Color = "#FFD700", TrustMultiplier = 1.2M },
            new() { Level = 4, Title = "Super Star", RequiredStars = 25000, Color = "#FFA500", TrustMultiplier = 1.3M },
            new() { Level = 5, Title = "Mega Star", RequiredStars = 100000, Color = "#FF69B4", TrustMultiplier = 1.5M }
        });
    }

    private async Task<List<StarsAchievementConfig>> GetAllAchievementConfigsAsync()
    {
        // В реальной реализации эти данные брались бы из конфигурации или базы
        return await Task.FromResult(new List<StarsAchievementConfig>
        {
            new() { Id = "first_gift", Name = "First Gift", RequiredStars = 100, ActivityType = MyTelegram.Domain.Shared.StarsRating.StarsActivityType.GiftSent },
            new() { Id = "generous_giver", Name = "Generous Giver", RequiredStars = 1000, ActivityType = MyTelegram.Domain.Shared.StarsRating.StarsActivityType.GiftSent, RequiredCount = 10 },
            new() { Id = "active_user", Name = "Active User", RequiredStars = 500, ActivityType = MyTelegram.Domain.Shared.StarsRating.StarsActivityType.MessageSent, RequiredCount = 100 }
        });
    }

    private async Task<List<UserStarsRating>> GetAllActiveUsersAsync()
    {
        // TODO: Implement GetAllActiveUsersQuery or use alternative approach
        // var query = new GetAllActiveUsersQuery();
        // var users = await queryProcessor.ProcessAsync(query);
        // return users?.ToList() ?? new List<UserStarsRating>();
        return new List<UserStarsRating>();
    }

    private async Task<long> GetStarsInPeriodAsync(long userId, DateTime periodStart, DateTime periodEnd)
    {
        var query = new GetStarsInPeriodQuery(userId, periodStart, periodEnd);
        var transactions = await queryProcessor.ProcessAsync(query);
        return transactions?.Sum(t => t.Amount) ?? 0;
    }

    private async Task<string> GetUserNameAsync(long userId)
    {
        var user = await userAppService.GetAsync(userId);
        return user?.UserName ?? $"User_{userId}";
    }

    private static (DateTime start, DateTime end) GetPeriodDates(MyTelegram.Domain.Shared.StarsRating.LeaderboardPeriod period)
    {
        var now = DateTime.UtcNow;
        return period switch
        {
            MyTelegram.Domain.Shared.StarsRating.LeaderboardPeriod.Daily => (now.Date, now.Date.AddDays(1)),
            MyTelegram.Domain.Shared.StarsRating.LeaderboardPeriod.Weekly => (now.AddDays(-7), now),
            MyTelegram.Domain.Shared.StarsRating.LeaderboardPeriod.Monthly => (now.AddDays(-30), now),
            MyTelegram.Domain.Shared.StarsRating.LeaderboardPeriod.Yearly => (now.AddDays(-365), now),
            _ => (DateTime.MinValue, now)
        };
    }

    private async Task<bool> IsAdminAsync()
    {
        // В реальной реализации здесь проверялись бы права администратора
        return await Task.FromResult(true); // Заглушка
    }

    // Вспомогательные методы для маппинга перечислений
    private static MyTelegram.Domain.Shared.StarsRating.StarsActivityType MapActivityType(StarsActivityType type)
    {
        return type switch
        {
            StarsActivityType.GiftSent => MyTelegram.Domain.Shared.StarsRating.StarsActivityType.GiftSent,
            StarsActivityType.GiftReceived => MyTelegram.Domain.Shared.StarsRating.StarsActivityType.GiftReceived,
            StarsActivityType.MessageSent => MyTelegram.Domain.Shared.StarsRating.StarsActivityType.MessageSent,
            StarsActivityType.PostSuggested => MyTelegram.Domain.Shared.StarsRating.StarsActivityType.PostSuggested,
            StarsActivityType.MessageBoosted => MyTelegram.Domain.Shared.StarsRating.StarsActivityType.BoostReceived,
            StarsActivityType.PostPublished => MyTelegram.Domain.Shared.StarsRating.StarsActivityType.PostPublished,
            StarsActivityType.PaymentMade => MyTelegram.Domain.Shared.StarsRating.StarsActivityType.PaymentMade,
            StarsActivityType.PaymentReceived => MyTelegram.Domain.Shared.StarsRating.StarsActivityType.PaymentReceived,
            StarsActivityType.BoostReceived => MyTelegram.Domain.Shared.StarsRating.StarsActivityType.BoostReceived,
            StarsActivityType.SubscriptionPaid => MyTelegram.Domain.Shared.StarsRating.StarsActivityType.SubscriptionPaid,
            StarsActivityType.ManualUpdate => MyTelegram.Domain.Shared.StarsRating.StarsActivityType.ManualUpdate,
            StarsActivityType.DonationMade => MyTelegram.Domain.Shared.StarsRating.StarsActivityType.DonationMade,
            StarsActivityType.AchievementUnlocked => MyTelegram.Domain.Shared.StarsRating.StarsActivityType.AchievementUnlocked,
            StarsActivityType.LevelUp => MyTelegram.Domain.Shared.StarsRating.StarsActivityType.LevelUp,
            _ => MyTelegram.Domain.Shared.StarsRating.StarsActivityType.ManualUpdate
        };
    }
    
    private static MyTelegram.Domain.Shared.StarsRating.LeaderboardType MapLeaderboardType(LeaderboardType type)
    {
        return type switch
        {
            LeaderboardType.Global => MyTelegram.Domain.Shared.StarsRating.LeaderboardType.Global,
            LeaderboardType.Regional => MyTelegram.Domain.Shared.StarsRating.LeaderboardType.Country,
            LeaderboardType.Country => MyTelegram.Domain.Shared.StarsRating.LeaderboardType.Country,
            LeaderboardType.City => MyTelegram.Domain.Shared.StarsRating.LeaderboardType.City,
            LeaderboardType.Channel => MyTelegram.Domain.Shared.StarsRating.LeaderboardType.Channel,
            LeaderboardType.AgeGroup => MyTelegram.Domain.Shared.StarsRating.LeaderboardType.AgeGroup,
            LeaderboardType.Interest => MyTelegram.Domain.Shared.StarsRating.LeaderboardType.Interest,
            _ => MyTelegram.Domain.Shared.StarsRating.LeaderboardType.Global
        };
    }
    
    private static MyTelegram.Domain.Shared.StarsRating.LeaderboardPeriod MapLeaderboardPeriod(LeaderboardPeriod period)
    {
        return period switch
        {
            LeaderboardPeriod.Daily => MyTelegram.Domain.Shared.StarsRating.LeaderboardPeriod.Daily,
            LeaderboardPeriod.Weekly => MyTelegram.Domain.Shared.StarsRating.LeaderboardPeriod.Weekly,
            LeaderboardPeriod.Monthly => MyTelegram.Domain.Shared.StarsRating.LeaderboardPeriod.Monthly,
            LeaderboardPeriod.Yearly => MyTelegram.Domain.Shared.StarsRating.LeaderboardPeriod.Yearly,
            LeaderboardPeriod.AllTime => MyTelegram.Domain.Shared.StarsRating.LeaderboardPeriod.AllTime,
            _ => MyTelegram.Domain.Shared.StarsRating.LeaderboardPeriod.AllTime
        };
    }
}

public class CreateStarsRatingCommand : Command<UserAggregate, UserId>
{
    public CreateStarsRatingCommand(UserId userId) : base(userId) { }
    
    public long UserId { get; set; }
    public UserStarsRating Rating { get; set; } = new();
    public DateTime CreatedAt { get; set; }
}

public class UpdateStarsRatingCommand : Command<UserAggregate, UserId>
{
    public UpdateStarsRatingCommand(UserId userId) : base(userId) { }
    
    public long UserId { get; set; }
    public long TotalStarsSpent { get; set; }
    public long TotalStarsReceived { get; set; }
    public int StarsLevel { get; set; }
    public string LevelTitle { get; set; } = string.Empty;
    public long StarsToNextLevel { get; set; }
    public DateTime UpdatedAt { get; set; }
    public double TrustScore { get; set; }
}

public class ConfigureStarsLevelCommand : Command<UserAggregate, UserId>
{
    public ConfigureStarsLevelCommand(UserId userId) : base(userId) { }
    
    public int Level { get; set; }
    public string Title { get; set; } = string.Empty;
    public long RequiredStars { get; set; }
    public string Color { get; set; } = string.Empty;
    public string Icon { get; set; } = string.Empty;
    public List<string> Benefits { get; set; } = new();
    public bool IsSpecial { get; set; }
    public string Badge { get; set; } = string.Empty;
    public double TrustMultiplier { get; set; }
    public DateTime ConfiguredAt { get; set; }
}

public class AddStarsAchievementCommand : Command<UserAggregate, UserId>
{
    public AddStarsAchievementCommand(UserId userId) : base(userId) { }
    
    public string AchievementId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Icon { get; set; } = string.Empty;
    public long RequiredStars { get; set; }
    public int Points { get; set; }
    public bool IsPublic { get; set; }
    public bool IsRare { get; set; }
    public string Category { get; set; } = string.Empty;
    public MyTelegram.Domain.Shared.StarsRating.StarsActivityType ActivityType { get; set; }
    public int RequiredCount { get; set; }
    public string Condition { get; set; } = string.Empty;
    public DateTime AddedAt { get; set; }
}

public class AwardStarsAchievementCommand : Command<UserAggregate, UserId>
{
    public AwardStarsAchievementCommand(UserId userId) : base(userId) { }
    
    public long UserId { get; set; }
    public MyTelegram.Domain.Shared.StarsRating.StarsAchievement Achievement { get; set; } = new();
    public DateTime AwardedAt { get; set; }
}

public class RecordStarsActivityCommand : Command<UserAggregate, UserId>
{
    public RecordStarsActivityCommand(UserId userId) : base(userId) { }
    
    public MyTelegram.Domain.Shared.StarsRating.StarsActivity Activity { get; set; } = new();
    public DateTime RecordedAt { get; set; }
}

public class UpdateTrustScoreCommand : Command<UserAggregate, UserId>
{
    public UpdateTrustScoreCommand(UserId userId) : base(userId) { }
    
    public long UserId { get; set; }
    public double TrustScore { get; set; }
    public DateTime CalculatedAt { get; set; }
    public List<string> Factors { get; set; } = new();
}

public class UpdateStarsLeaderboardCommand : Command<UserAggregate, UserId>
{
    public UpdateStarsLeaderboardCommand(UserId userId) : base(userId) { }
    
    public StarsLeaderboard Leaderboard { get; set; } = new();
    public DateTime UpdatedAt { get; set; }
}
