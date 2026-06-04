using Microsoft.Extensions.Logging;
using MyTelegram.Domain.Shared.Affiliate;
using MyTelegram.Domain.Shared.Business;
using MyTelegram.Domain.Aggregates.Channel;
using EventFlow.Aggregates.ExecutionResults;
using EventFlow.Commands;

namespace MyTelegram.Messenger.Services.Impl;

/// <summary>
/// Interface for managing affiliate programs
/// </summary>
public interface IAffiliateAppService
{
    Task<MyTelegram.Domain.Shared.Affiliate.StarReferralProgram> CreateReferralProgramAsync(CreateReferralProgramRequest request);
    Task<MyTelegram.Domain.Shared.Affiliate.StarReferralProgram?> UpdateReferralProgramAsync(string programId, UpdateReferralProgramRequest request);
    Task<MyTelegram.Domain.Shared.Affiliate.StarReferralProgram?> GetReferralProgramAsync(long botId);
    Task<Affiliate> CreateAffiliateAsync(CreateAffiliateRequest request);
    Task<Affiliate?> GetAffiliateAsync(long affiliateId, long botId);
    Task<string> GenerateReferralLinkAsync(long affiliateId, long botId, string? campaign = null);
    Task<Referral> ProcessReferralAsync(ReferralRequest request);
    Task<AffiliateCommission> CreateCommissionAsync(CreateCommissionRequest request);
    Task<AffiliateStats> GetAffiliateStatsAsync(long affiliateId, long botId, DateTime? from = null, DateTime? to = null);
    Task<List<Referral>> GetAffiliateReferralsAsync(long affiliateId, long botId, int offset = 0, int limit = 50);
    Task<List<AffiliateCommission>> GetAffiliateCommissionsAsync(long affiliateId, long botId, int offset = 0, int limit = 50);
    Task<AffiliatePayout> RequestPayoutAsync(long affiliateId, long botId, long amount);
    Task<List<AffiliatePayout>> GetPayoutsAsync(long affiliateId, long botId, int offset = 0, int limit = 50);
    Task<bool> UpdateCommissionStatusAsync(string commissionId, CommissionStatus status, string? reason = null);
    Task<MyTelegram.Domain.Shared.Affiliate.AffiliateLink> CreateAffiliateLinkAsync(CreateAffiliateLinkRequest request);
    Task<List<MyTelegram.Domain.Shared.Affiliate.AffiliateLink>> GetAffiliateLinksAsync(long affiliateId, long botId);
    Task<bool> TrackClickAsync(ClickTrackingRequest request);
}

/// <summary>
/// Service for managing affiliate programs functionality
/// </summary>
public sealed class AffiliateAppService(
    ILogger<AffiliateAppService> logger,
    IQueryProcessor queryProcessor,
    ICommandBus commandBus,
    IUserAppService userAppService) : IAffiliateAppService
{
    public async Task<MyTelegram.Domain.Shared.Affiliate.StarReferralProgram> CreateReferralProgramAsync(CreateReferralProgramRequest request)
    {
        logger.LogInformation("Creating referral program for bot {BotId}", request.BotId);

        // Validate user owns the bot
        var bot = await userAppService.GetAsync(request.BotId);
        if (bot == null || !await IsBotOwnerAsync(request.CreatorId, request.BotId))
        {
            throw new UnauthorizedAccessException("User is not bot owner");
        }

        // Validate commission rate (max 50% = 500 permille)
        if (request.CommissionPermille <= 0 || request.CommissionPermille > 500)
        {
            throw new ArgumentException("Commission permille must be between 1 and 500");
        }

        // Check if program already exists
        var existingProgram = await GetReferralProgramAsync(request.BotId);
        if (existingProgram != null)
        {
            throw new InvalidOperationException("Referral program already exists for this bot");
        }

        var command = new CreateReferralProgramCommand(new ChannelId(request.BotId.ToString()))
        {
            BotId = request.BotId,
            CreatorId = request.CreatorId,
            CommissionPermille = request.CommissionPermille,
            DurationMonths = request.DurationMonths,
            EndDate = DateTime.UtcNow.AddMonths(request.DurationMonths),
            DailyRevenuePerUser = request.DailyRevenuePerUser,
            Description = request.Description,
            TermsUrl = request.TermsUrl,
            Settings = request.Settings ?? new AffiliateSettings(),
            CreatedAt = DateTime.UtcNow
        };

        await commandBus.PublishAsync<ChannelAggregate, ChannelId, IExecutionResult>(command, CancellationToken.None);

        var program = new StarReferralProgram
        {
            Id = Guid.NewGuid().ToString(),
            BotId = request.BotId,
            CreatorId = request.CreatorId,
            CommissionPermille = request.CommissionPermille,
            DurationMonths = request.DurationMonths,
            EndDate = command.EndDate,
            DailyRevenuePerUser = request.DailyRevenuePerUser,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            Description = request.Description,
            TermsUrl = request.TermsUrl,
            Settings = command.Settings
        };

        logger.LogInformation("Referral program created successfully: {ProgramId} for bot {BotId}", program.Id, request.BotId);
        return program;
    }

    public async Task<MyTelegram.Domain.Shared.Affiliate.StarReferralProgram?> UpdateReferralProgramAsync(string programId, UpdateReferralProgramRequest request)
    {
        logger.LogInformation("Updating referral program {ProgramId}", programId);

        var program = await queryProcessor.ProcessAsync(new GetReferralProgramQuery(programId));
        if (program == null)
        {
            return null;
        }

        // Validate user owns the program
        if (!await IsBotOwnerAsync(request.UpdatedBy, program.BotId))
        {
            throw new UnauthorizedAccessException("User is not bot owner");
        }

        var command = new UpdateReferralProgramCommand(new ChannelId(program.BotId.ToString()))
        {
            ProgramId = programId,
            CommissionPermille = request.CommissionPermille ?? program.CommissionPermille,
            DurationMonths = request.DurationMonths ?? program.DurationMonths,
            DailyRevenuePerUser = request.DailyRevenuePerUser ?? program.DailyRevenuePerUser,
            Description = request.Description ?? program.Description,
            TermsUrl = request.TermsUrl ?? program.TermsUrl,
            Settings = request.Settings ?? program.Settings,
            UpdatedAt = DateTime.UtcNow
        };

        await commandBus.PublishAsync<ChannelAggregate, ChannelId, IExecutionResult>(command, CancellationToken.None);

        logger.LogInformation("Referral program updated successfully: {ProgramId}", programId);
        return await GetReferralProgramAsync(program.BotId);
    }

    public async Task<MyTelegram.Domain.Shared.Affiliate.StarReferralProgram?> GetReferralProgramAsync(long botId)
    {
        var program = await queryProcessor.ProcessAsync(new GetReferralProgramByBotIdQuery(botId));
        return program;
    }

    public async Task<Affiliate> CreateAffiliateAsync(CreateAffiliateRequest request)
    {
        logger.LogInformation("Creating affiliate for user {UserId} in bot {BotId}", request.UserId, request.BotId);

        // Check if user is already an affiliate for this bot
        var existingAffiliate = await GetAffiliateAsync(request.UserId, request.BotId);
        if (existingAffiliate != null)
        {
            throw new InvalidOperationException("User is already an affiliate for this bot");
        }

        var program = await GetReferralProgramAsync(request.BotId);
        if (program == null || !program.IsActive)
        {
            throw new InvalidOperationException("No active referral program for this bot");
        }

        var referralCode = GenerateReferralCode(request.UserId, request.BotId);
        var referralUrl = GenerateReferralUrl(referralCode, request.BotId, request.Campaign);

        var command = new CreateAffiliateCommand(new ChannelId(request.BotId.ToString()))
        {
            AffiliateId = request.UserId,
            BotId = request.BotId,
            ReferralCode = referralCode,
            CustomReferralUrl = referralUrl,
            JoinedAt = DateTime.UtcNow,
            IsActive = true,
            IsVerified = program.Settings.RequireVerification ? false : true
        };

        await commandBus.PublishAsync<ChannelAggregate, ChannelId, IExecutionResult>(command, CancellationToken.None);

        var affiliate = new Affiliate
        {
            Id = Guid.NewGuid().ToString(),
            AffiliateId = request.UserId,
            BotId = request.BotId,
            ReferralCode = referralCode,
            CustomReferralUrl = referralUrl,
            CurrentTier = 0,
            JoinedAt = DateTime.UtcNow,
            IsActive = true,
            IsVerified = command.IsVerified
        };

        logger.LogInformation("Affiliate created successfully: {AffiliateId} for bot {BotId}", request.UserId, request.BotId);
        return affiliate;
    }

    public async Task<Affiliate?> GetAffiliateAsync(long affiliateId, long botId)
    {
        var affiliate = await queryProcessor.ProcessAsync(new GetAffiliateQuery(affiliateId, botId));
        return affiliate;
    }

    public async Task<string> GenerateReferralLinkAsync(long affiliateId, long botId, string? campaign = null)
    {
        var affiliate = await GetAffiliateAsync(affiliateId, botId);
        if (affiliate == null)
        {
            throw new ArgumentException("Affiliate not found");
        }

        var referralUrl = GenerateReferralUrl(affiliate.ReferralCode, botId, campaign);
        
        logger.LogInformation("Generated referral link for affiliate {AffiliateId}: {Url}", affiliateId, referralUrl);
        return referralUrl;
    }

    public async Task<Referral> ProcessReferralAsync(ReferralRequest request)
    {
        logger.LogInformation("Processing referral for bot {BotId} with code {ReferralCode}", 
            request.BotId, request.ReferralCode);

        // Find the affiliate by referral code
        var affiliate = await queryProcessor.ProcessAsync(
            new GetAffiliateByReferralCodeQuery(request.ReferralCode, request.BotId));

        if (affiliate == null || !affiliate.IsActive)
        {
            throw new ArgumentException("Invalid or inactive referral code");
        }

        // Check if user is already referred
        var existingReferral = await queryProcessor.ProcessAsync(
            new GetReferralByUserQuery(request.ReferredUserId));

        if (existingReferral != null)
        {
            throw new InvalidOperationException("User is already referred");
        }

        var referralId = Guid.NewGuid().ToString();
        var command = new CreateReferralCommand(new ChannelId(request.BotId.ToString()))
        {
            ReferralId = referralId,
            AffiliateId = affiliate.AffiliateId,
            ReferredUserId = request.ReferredUserId,
            BotId = request.BotId,
            ReferralCode = request.ReferralCode,
            ReferredAt = DateTime.UtcNow,
            Source = request.Source,
            Campaign = request.Campaign,
            Status = ReferralStatus.Pending
        };

        await commandBus.PublishAsync<ChannelAggregate, ChannelId, IExecutionResult>(command, CancellationToken.None);

        // Update affiliate stats
        await UpdateAffiliateStatsAsync(affiliate.AffiliateId, request.BotId);

        var referral = new Referral
        {
            Id = referralId,
            AffiliateId = affiliate.AffiliateId.ToString(),
            ReferredUserId = request.ReferredUserId,
            BotId = request.BotId,
            ReferralCode = request.ReferralCode,
            ReferredAt = DateTime.UtcNow,
            Source = request.Source,
            Campaign = request.Campaign,
            Status = ReferralStatus.Pending
        };

        logger.LogInformation("Referral processed successfully: {ReferralId}", referralId);
        return referral;
    }

    public async Task<AffiliateCommission> CreateCommissionAsync(CreateCommissionRequest request)
    {
        logger.LogInformation("Creating commission for affiliate {AffiliateId}, amount {Amount}", 
            request.AffiliateId, request.Amount);

        var affiliate = await GetAffiliateAsync(request.AffiliateId, request.BotId);
        if (affiliate == null)
        {
            throw new ArgumentException("Affiliate not found");
        }

        var program = await GetReferralProgramAsync(request.BotId);
        if (program == null)
        {
            throw new ArgumentException("No referral program found");
        }

        // Calculate commission
        var commissionAmount = CalculateCommission(request.Amount, program.CommissionPermille, affiliate.CurrentTier);

        var command = new CreateCommissionCommand(new ChannelId(request.BotId.ToString()))
        {
            AffiliateId = request.AffiliateId,
            ReferralId = request.ReferralId,
            BotId = request.BotId,
            Amount = commissionAmount,
            CommissionPermille = program.CommissionPermille,
            CreatedAt = DateTime.UtcNow,
            PurchaseId = request.PurchaseId,
            OriginalAmount = request.Amount,
            Description = request.Description
        };

        await commandBus.PublishAsync<ChannelAggregate, ChannelId, IExecutionResult>(command, CancellationToken.None);

        var commission = new AffiliateCommission
        {
            Id = Guid.NewGuid().ToString(),
            AffiliateId = request.AffiliateId,
            ReferralId = request.ReferralId,
            BotId = request.BotId,
            Amount = commissionAmount,
            CommissionPermille = program.CommissionPermille,
            CreatedAt = DateTime.UtcNow,
            PurchaseId = request.PurchaseId,
            OriginalAmount = request.Amount,
            // Status = CommissionStatus.Pending
        };

        logger.LogInformation("Commission created successfully: {CommissionId}, amount {Amount}", 
            commission.Id, commissionAmount);
        return commission;
    }

    public async Task<AffiliateStats> GetAffiliateStatsAsync(long affiliateId, long botId, DateTime? from = null, DateTime? to = null)
    {
        logger.LogInformation("Getting affiliate stats for user {UserId} in bot {BotId}", affiliateId, botId);

        var query = new GetAffiliateStatsQuery(affiliateId, botId)
        {
            PeriodStart = from ?? DateTime.UtcNow.AddDays(-30),
            PeriodEnd = to ?? DateTime.UtcNow
        };

        var stats = await queryProcessor.ProcessAsync(query);
        
        return stats ?? new AffiliateStats
        {
            AffiliateId = affiliateId,
            BotId = botId,
            PeriodStart = query.PeriodStart,
            PeriodEnd = query.PeriodEnd
        };
    }

    public async Task<List<Referral>> GetAffiliateReferralsAsync(long affiliateId, long botId, int offset = 0, int limit = 50)
    {
        var referrals = await queryProcessor.ProcessAsync(
            new GetAffiliateReferralsQuery(affiliateId, botId, offset, limit));

        return referrals?.ToList() ?? new List<Referral>();
    }

    public async Task<List<AffiliateCommission>> GetAffiliateCommissionsAsync(long affiliateId, long botId, int offset = 0, int limit = 50)
    {
        var commissions = await queryProcessor.ProcessAsync(
            new GetAffiliateCommissionsQuery(affiliateId, botId, offset, limit));

        return commissions?.ToList() ?? new List<AffiliateCommission>();
    }

    public async Task<AffiliatePayout> RequestPayoutAsync(long affiliateId, long botId, long amount)
    {
        logger.LogInformation("Requesting payout for affiliate {AffiliateId}, amount {Amount}", affiliateId, amount);

        var affiliate = await GetAffiliateAsync(affiliateId, botId);
        if (affiliate == null)
        {
            throw new ArgumentException("Affiliate not found");
        }

        // Check if has sufficient approved commission
        var approvedCommissions = await queryProcessor.ProcessAsync(
            new GetAffiliateApprovedCommissionsQuery(affiliateId, botId));

        if (approvedCommissions?.Sum(c => c.Amount) < amount)
        {
            throw new InvalidOperationException("Insufficient approved commission");
        }

        var command = new CreatePayoutCommand(new ChannelId(botId.ToString()))
        {
            AffiliateId = affiliateId,
            BotId = botId,
            Amount = amount,
            RequestedAt = DateTime.UtcNow,
            Status = PayoutStatus.Pending
        };

        await commandBus.PublishAsync<ChannelAggregate, ChannelId, IExecutionResult>(command, CancellationToken.None);

        var payout = new AffiliatePayout
        {
            Id = Guid.NewGuid().ToString(),
            AffiliateId = affiliateId,
            BotId = botId,
            Amount = amount,
            RequestedAt = DateTime.UtcNow,
            Status = PayoutStatus.Pending
        };

        logger.LogInformation("Payout request created: {PayoutId}", payout.Id);
        return payout;
    }

    public async Task<List<AffiliatePayout>> GetPayoutsAsync(long affiliateId, long botId, int offset = 0, int limit = 50)
    {
        var payouts = await queryProcessor.ProcessAsync(
            new GetAffiliatePayoutsQuery(affiliateId, botId, offset, limit));

        return payouts?.ToList() ?? new List<AffiliatePayout>();
    }

    public async Task<bool> UpdateCommissionStatusAsync(string commissionId, CommissionStatus status, string? reason = null)
    {
        logger.LogInformation("Updating commission {CommissionId} status to {Status}", commissionId, status);

        // Note: Need to get commission to extract botId/channelId
        // For now, using a placeholder - this needs proper implementation
        var channelId = ChannelId.With("0"); // TODO: Get actual channelId from commission
        
        var command = new UpdateCommissionStatusCommand(channelId)
        {
            CommissionId = commissionId,
            Status = status,
            Reason = reason,
            UpdatedAt = DateTime.UtcNow
        };

        await commandBus.PublishAsync<ChannelAggregate, ChannelId, IExecutionResult>(command, CancellationToken.None);
        return true;
    }

    public async Task<MyTelegram.Domain.Shared.Affiliate.AffiliateLink> CreateAffiliateLinkAsync(CreateAffiliateLinkRequest request)
    {
        logger.LogInformation("Creating affiliate link for affiliate {AffiliateId}", request.AffiliateId);

        var affiliate = await GetAffiliateAsync(request.AffiliateId, request.BotId);
        if (affiliate == null)
        {
            throw new ArgumentException("Affiliate not found");
        }

        var linkId = Guid.NewGuid().ToString();
        var customUrl = GenerateCustomAffiliateUrl(affiliate.ReferralCode, request.BotId, request.Campaign);
        var channelId = ChannelId.With(request.BotId.ToString());

        var command = new CreateAffiliateLinkCommand(channelId)
        {
            LinkId = linkId,
            AffiliateId = request.AffiliateId,
            BotId = request.BotId,
            Url = customUrl,
            Campaign = request.Campaign ?? "default",
            Description = request.Description,
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = request.ExpiresAt
        };

        await commandBus.PublishAsync<ChannelAggregate, ChannelId, IExecutionResult>(command, CancellationToken.None);

        var link = new MyTelegram.Domain.Shared.Affiliate.AffiliateLink
        {
            Id = linkId,
            AffiliateId = request.AffiliateId,
            BotId = request.BotId,
            Url = customUrl,
            Campaign = request.Campaign ?? "default",
            Description = request.Description,
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = request.ExpiresAt,
            IsActive = true
        };

        logger.LogInformation("Affiliate link created: {LinkId}", linkId);
        return link;
    }

    public async Task<List<MyTelegram.Domain.Shared.Affiliate.AffiliateLink>> GetAffiliateLinksAsync(long affiliateId, long botId)
    {
        var links = await queryProcessor.ProcessAsync(
            new GetAffiliateLinksQuery(affiliateId, botId));

        return new List<MyTelegram.Domain.Shared.Affiliate.AffiliateLink>();
    }

    public async Task<bool> TrackClickAsync(ClickTrackingRequest request)
    {
        logger.LogInformation("Tracking affiliate click: {LinkId} from {Ip}", request.LinkId, request.IpAddress);

        var channelId = ChannelId.With(request.BotId.ToString());
        var command = new TrackAffiliateClickCommand(channelId)
        {
            ClickId = Guid.NewGuid().ToString(),
            LinkId = request.LinkId,
            AffiliateId = request.AffiliateId,
            BotId = request.BotId,
            IpAddress = request.IpAddress,
            UserAgent = request.UserAgent,
            Country = request.Country,
            Referer = request.Referer,
            ClickedAt = DateTime.UtcNow
        };

        await commandBus.PublishAsync<ChannelAggregate, ChannelId, IExecutionResult>(command, CancellationToken.None);
        return true;
    }

    private static string GenerateReferralCode(long userId, long botId)
    {
        var timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        var hash = $"{userId}{botId}{timestamp}".GetHashCode();
        return Convert.ToBase64String(BitConverter.GetBytes(hash)).Replace("+", "-").Replace("/", "_").Substring(0, 8);
    }

    private static string GenerateReferralUrl(string referralCode, long botId, string? campaign)
    {
        var baseUrl = $"https://t.me/{referralCode}";
        return string.IsNullOrEmpty(campaign) ? baseUrl : $"{baseUrl}?c={campaign}";
    }

    private static string GenerateCustomAffiliateUrl(string referralCode, long botId, string? campaign)
    {
        return GenerateReferralUrl(referralCode, botId, campaign);
    }

    private static long CalculateCommission(long amount, int commissionPermille, int tier)
    {
        var baseCommission = (amount * commissionPermille) / 1000;
        // Add tier bonus if applicable
        return baseCommission; // Simplified for now
    }

    private async Task UpdateAffiliateStatsAsync(long affiliateId, long botId)
    {
        // Implementation would update affiliate's total referrals and other stats
        await Task.CompletedTask;
    }

    private async Task<bool> IsBotOwnerAsync(long userId, long botId)
    {
        // Implementation needed to check if user owns the bot
        return true; // Placeholder
    }
}

// Request and command classes
public class CreateReferralProgramRequest
{
    public long BotId { get; set; }
    public long CreatorId { get; set; }
    public int CommissionPermille { get; set; }
    public int DurationMonths { get; set; }
    public long DailyRevenuePerUser { get; set; }
    public string? Description { get; set; }
    public string? TermsUrl { get; set; }
    public AffiliateSettings? Settings { get; set; }
}

public class UpdateReferralProgramRequest
{
    public long UpdatedBy { get; set; }
    public int? CommissionPermille { get; set; }
    public int? DurationMonths { get; set; }
    public long? DailyRevenuePerUser { get; set; }
    public string? Description { get; set; }
    public string? TermsUrl { get; set; }
    public AffiliateSettings? Settings { get; set; }
}

public class CreateAffiliateRequest
{
    public long UserId { get; set; }
    public long BotId { get; set; }
    public string? Campaign { get; set; }
}

public class ReferralRequest
{
    public long ReferredUserId { get; set; }
    public long BotId { get; set; }
    public string ReferralCode { get; set; } = string.Empty;
    public string? Source { get; set; }
    public string? Campaign { get; set; }
}

public class CreateCommissionRequest
{
    public long AffiliateId { get; set; }
    public string ReferralId { get; set; } = string.Empty;
    public long BotId { get; set; }
    public long Amount { get; set; }
    public string? PurchaseId { get; set; }
    public string? Description { get; set; }
}

public class CreateAffiliateLinkRequest
{
    public long AffiliateId { get; set; }
    public long BotId { get; set; }
    public string? Campaign { get; set; }
    public string? Description { get; set; }
    public DateTime? ExpiresAt { get; set; }
}

public class ClickTrackingRequest
{
    public string LinkId { get; set; } = string.Empty;
    public long AffiliateId { get; set; }
    public long BotId { get; set; }
    public string IpAddress { get; set; } = string.Empty;
    public string UserAgent { get; set; } = string.Empty;
    public string? Country { get; set; }
    public string? Referer { get; set; }
}

// Query and command classes for affiliate operations
public class GetAffiliateByReferralCodeQuery : IQuery<Affiliate>
{
    public string ReferralCode { get; set; } = string.Empty;
    public long BotId { get; set; }
    
    public GetAffiliateByReferralCodeQuery(string referralCode, long botId)
    {
        ReferralCode = referralCode;
        BotId = botId;
    }
}

public class CreateReferralProgramCommand : Command<ChannelAggregate, ChannelId>
{
    public CreateReferralProgramCommand(ChannelId channelId) : base(channelId) { }
    
    public long BotId { get; set; }
    public long CreatorId { get; set; }
    public int CommissionPermille { get; set; }
    public int DurationMonths { get; set; }
    public DateTime EndDate { get; set; }
    public long DailyRevenuePerUser { get; set; }
    public string? Description { get; set; }
    public string? TermsUrl { get; set; }
    public AffiliateSettings Settings { get; set; } = new();
    public DateTime CreatedAt { get; set; }
}

public class UpdateReferralProgramCommand : Command<ChannelAggregate, ChannelId>
{
    public UpdateReferralProgramCommand(ChannelId channelId) : base(channelId) { }
    
    public string ProgramId { get; set; } = string.Empty;
    public int CommissionPermille { get; set; }
    public int DurationMonths { get; set; }
    public long DailyRevenuePerUser { get; set; }
    public string? Description { get; set; }
    public string? TermsUrl { get; set; }
    public AffiliateSettings Settings { get; set; } = new();
    public DateTime UpdatedAt { get; set; }
}

public class CreateAffiliateCommand : Command<ChannelAggregate, ChannelId>
{
    public CreateAffiliateCommand(ChannelId channelId) : base(channelId) { }
    
    public long AffiliateId { get; set; }
    public long BotId { get; set; }
    public string ReferralCode { get; set; } = string.Empty;
    public string CustomReferralUrl { get; set; } = string.Empty;
    public DateTime JoinedAt { get; set; }
    public bool IsActive { get; set; }
    public bool IsVerified { get; set; }
}

public class CreateReferralCommand : Command<ChannelAggregate, ChannelId>
{
    public CreateReferralCommand(ChannelId channelId) : base(channelId) { }
    
    public string ReferralId { get; set; } = string.Empty;
    public long AffiliateId { get; set; }
    public long ReferredUserId { get; set; }
    public long BotId { get; set; }
    public string ReferralCode { get; set; } = string.Empty;
    public DateTime ReferredAt { get; set; }
    public string? Source { get; set; }
    public string? Campaign { get; set; }
    public ReferralStatus Status { get; set; }
}

public class CreateCommissionCommand : Command<ChannelAggregate, ChannelId>
{
    public CreateCommissionCommand(ChannelId channelId) : base(channelId) { }
    
    public string CommissionId { get; set; } = string.Empty;
    public long AffiliateId { get; set; }
    public string ReferralId { get; set; } = string.Empty;
    public long BotId { get; set; }
    public long Amount { get; set; }
    public int CommissionPermille { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? PurchaseId { get; set; }
    public long OriginalAmount { get; set; }
    public string? Description { get; set; }
}

public class CreatePayoutCommand : Command<ChannelAggregate, ChannelId>
{
    public CreatePayoutCommand(ChannelId channelId) : base(channelId) { }
    
    public string PayoutId { get; set; } = string.Empty;
    public long AffiliateId { get; set; }
    public long BotId { get; set; }
    public long Amount { get; set; }
    public DateTime RequestedAt { get; set; }
    public PayoutStatus Status { get; set; }
}

public class CreateAffiliateLinkCommand : Command<ChannelAggregate, ChannelId>
{
    public CreateAffiliateLinkCommand(ChannelId channelId) : base(channelId) { }
    
    public string LinkId { get; set; } = string.Empty;
    public long AffiliateId { get; set; }
    public long BotId { get; set; }
    public string Url { get; set; } = string.Empty;
    public string Campaign { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ExpiresAt { get; set; }
}

public class TrackAffiliateClickCommand : Command<ChannelAggregate, ChannelId>
{
    public TrackAffiliateClickCommand(ChannelId channelId) : base(channelId) { }
    
    public string ClickId { get; set; } = string.Empty;
    public string LinkId { get; set; } = string.Empty;
    public long AffiliateId { get; set; }
    public long BotId { get; set; }
    public string IpAddress { get; set; } = string.Empty;
    public string UserAgent { get; set; } = string.Empty;
    public string? Country { get; set; }
    public string? Referer { get; set; }
    public DateTime ClickedAt { get; set; }
}

public class UpdateCommissionStatusCommand : Command<ChannelAggregate, ChannelId>
{
    public UpdateCommissionStatusCommand(ChannelId channelId) : base(channelId) { }
    
    public string CommissionId { get; set; } = string.Empty;
    public CommissionStatus Status { get; set; }
    public string? Reason { get; set; }
    public DateTime UpdatedAt { get; set; }
}
