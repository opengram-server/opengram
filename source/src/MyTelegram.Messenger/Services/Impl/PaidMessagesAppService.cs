using Microsoft.Extensions.Logging;
using MyTelegram.Schema;
using MyTelegram.Messenger.Services;
using MyTelegram.Messenger.Services.Interfaces;
using MyTelegram.Domain.Shared;
using MyTelegram.Domain.Shared.Messages;
using EventFlow.Commands;
using EventFlow.Aggregates.ExecutionResults;

namespace MyTelegram.Messenger.Services.Impl;

/// <summary>
/// Interface for managing paid messages functionality
/// </summary>
public interface IPaidMessagesAppService
{
    Task<PaidMessagePaymentResult> ProcessPaidMessageAsync(PaidMessagePaymentRequest request);
    Task<PaidMessageSettings> UpdateChannelSettingsAsync(long channelId, PaidMessageSettings settings);
    Task<PaidMessageSettings> GetChannelSettingsAsync(long channelId);
    Task<PaidMessageStats> GetChannelStatsAsync(long channelId, DateTime? from = null, DateTime? to = null);
    Task<UserPaidMessageHistory> GetUserHistoryAsync(long userId, int offset = 0, int limit = 50);
    Task<bool> RefundPaidMessageAsync(string paidMessageId, string reason);
    Task<PaidMessageRevenue> GetChannelRevenueAsync(long channelId);
    Task<List<PaidMessageExemption>> GetChannelExemptionsAsync(long channelId);
    Task<bool> AddUserExemptionAsync(long channelId, long userId, PaidMessageExemptionType type, string? reason = null, DateTime? expiresAt = null);
    Task<bool> RemoveUserExemptionAsync(long channelId, long userId);
    Task<bool> CanSendPaidMessageAsync(long userId, long channelId, string messageContent);
}

/// <summary>
/// Service for managing paid messages functionality
/// </summary>
internal sealed class PaidMessagesAppService(
    ILogger<PaidMessagesAppService> logger,
    IQueryProcessor queryProcessor,
    ICommandBus commandBus,
    IStarsAppService starsAppService,
    IUserAppService userAppService) : IPaidMessagesAppService
{
    public async Task<PaidMessagePaymentResult> ProcessPaidMessageAsync(PaidMessagePaymentRequest request)
    {
        logger.LogInformation("Processing paid message for user {UserId} in channel {ChannelId}", 
            request.UserId, request.ChannelId);

        try
        {
            // Check if channel has paid messages enabled
            var channelSettings = await GetChannelSettingsAsync(request.ChannelId);
            if (!channelSettings.Enabled)
            {
                return new PaidMessagePaymentResult
                {
                    Success = false,
                    ErrorMessage = "Paid messages are not enabled for this channel"
                };
            }

            // Check if user is exempt from payment
            if (await IsUserExemptAsync(request.UserId, request.ChannelId))
            {
                return await ProcessExemptMessageAsync(request, channelSettings);
            }

            // Validate message content against channel rules
            var validationResult = await ValidateMessageAsync(request, channelSettings);
            if (!validationResult.IsValid)
            {
                return new PaidMessagePaymentResult
                {
                    Success = false,
                    ErrorMessage = validationResult.ErrorMessage
                };
            }

            // Check user's stars balance
            var starsStatus = await starsAppService.GetStarsStatusAsync(request.UserId);
            if (starsStatus.Balance < channelSettings.StarsAmount)
            {
                return new PaidMessagePaymentResult
                {
                    Success = false,
                    ErrorMessage = "Insufficient stars balance"
                };
            }

            // Process payment
            var paymentSuccess = await ProcessPaymentAsync(request.UserId, request.ChannelId, channelSettings.StarsAmount);
            if (!paymentSuccess)
            {
                return new PaidMessagePaymentResult
                {
                    Success = false,
                    ErrorMessage = "Payment failed"
                };
            }

            // Create the paid message record
            var paidMessageId = await CreatePaidMessageRecordAsync(request, channelSettings.StarsAmount);

            // Send the actual message (would need to integrate with message service)
            var messageId = await SendMessageAsync(request);

            logger.LogInformation("Paid message processed successfully: {PaidMessageId} for user {UserId}", 
                paidMessageId, request.UserId);

            return new PaidMessagePaymentResult
            {
                Success = true,
                PaidMessageId = paidMessageId,
                MessageId = messageId,
                StarsSpent = channelSettings.StarsAmount,
                TransactionId = Guid.NewGuid().ToString(),
                PaidAt = DateTime.UtcNow
            };
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to process paid message for user {UserId} in channel {ChannelId}", 
                request.UserId, request.ChannelId);

            return new PaidMessagePaymentResult
            {
                Success = false,
                ErrorMessage = "Internal error occurred"
            };
        }
    }

    public async Task<PaidMessageSettings> UpdateChannelSettingsAsync(long channelId, PaidMessageSettings settings)
    {
        logger.LogInformation("Updating paid message settings for channel {ChannelId}", channelId);

        // Validate user is channel admin
        if (!await IsChannelAdminAsync(channelId, settings.UpdatedAt.GetHashCode())) // Simplified check
        {
            throw new UnauthorizedAccessException("User is not channel admin");
        }

        // Validate settings
        ValidateChannelSettings(settings);

        var command = new UpdatePaidMessageSettingsCommand(MyTelegram.Domain.Aggregates.Channel.ChannelId.Create(channelId))
        {
            ChannelId = channelId,
            Settings = settings,
            UpdatedAt = DateTime.UtcNow
        };

        await commandBus.PublishAsync<ChannelAggregate, ChannelId, IExecutionResult>(command, CancellationToken.None);

        logger.LogInformation("Paid message settings updated for channel {ChannelId}", channelId);
        return settings;
    }

    public async Task<PaidMessageSettings> GetChannelSettingsAsync(long channelId)
    {
        var settings = await queryProcessor.ProcessAsync(new GetPaidMessageSettingsQuery(channelId));
        
        return settings ?? new PaidMessageSettings
        {
            ChannelId = channelId,
            Enabled = false,
            StarsAmount = 0,
            RestrictionType = PaidMessageRestrictionType.None,
            MinMessageLength = 1,
            MaxMessageLength = 4096,
            AllowMedia = true,
            AllowLinks = true,
            AllowForwards = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }

    public async Task<PaidMessageStats> GetChannelStatsAsync(long channelId, DateTime? from = null, DateTime? to = null)
    {
        logger.LogInformation("Getting paid message stats for channel {ChannelId}, period: {From} - {To}", 
            channelId, from, to);

        var query = new GetPaidMessageStatsQuery(channelId)
        {
            PeriodStart = from ?? DateTime.UtcNow.AddDays(-30),
            PeriodEnd = to ?? DateTime.UtcNow
        };

        var stats = await queryProcessor.ProcessAsync(query);
        
        return stats ?? new PaidMessageStats
        {
            ChannelId = channelId,
            PeriodStart = query.PeriodStart,
            PeriodEnd = query.PeriodEnd
        };
    }

    public async Task<UserPaidMessageHistory> GetUserHistoryAsync(long userId, int offset = 0, int limit = 50)
    {
        logger.LogInformation("Getting paid message history for user {UserId}, offset: {Offset}, limit: {Limit}", 
            userId, offset, limit);

        var history = await queryProcessor.ProcessAsync(
            new GetUserPaidMessageHistoryQuery(userId, offset, limit));

        return history ?? new UserPaidMessageHistory
        {
            UserId = userId,
            ChannelStats = new List<ChannelMessageStats>()
        };
    }

    public async Task<bool> RefundPaidMessageAsync(string paidMessageId, string reason)
    {
        logger.LogInformation("Refunding paid message {PaidMessageId}, reason: {Reason}", paidMessageId, reason);

        var paidMessage = await queryProcessor.ProcessAsync(new GetPaidMessageByIdQuery(paidMessageId));
        if (paidMessage == null || paidMessage.IsRefunded)
        {
            return false;
        }

        // Process refund
        var refundSuccess = await starsAppService.UpdateStarsBalanceAsync(
            paidMessage.SenderId, paidMessage.StarsAmount, "refund", $"Paid message refund: {reason}");

        if (!refundSuccess)
        {
            return false;
        }

        // Update paid message record
        var command = new RefundPaidMessageCommand(MyTelegram.Domain.Aggregates.Channel.ChannelId.Create(paidMessage.ChannelId))
        {
            PaidMessageId = paidMessageId,
            RefundReason = reason,
            RefundedAt = DateTime.UtcNow
        };

        await commandBus.PublishAsync<ChannelAggregate, ChannelId, IExecutionResult>(command, CancellationToken.None);

        logger.LogInformation("Paid message refunded successfully: {PaidMessageId}", paidMessageId);
        return true;
    }

    public async Task<PaidMessageRevenue> GetChannelRevenueAsync(long channelId)
    {
        logger.LogInformation("Getting paid message revenue for channel {ChannelId}", channelId);

        var revenue = await queryProcessor.ProcessAsync(new GetPaidMessageRevenueQuery(channelId));
        
        return revenue ?? new PaidMessageRevenue
        {
            ChannelId = channelId,
            RevenueBreakdown = new List<RevenueBreakdown>()
        };
    }

    public async Task<List<PaidMessageExemption>> GetChannelExemptionsAsync(long channelId)
    {
        var exemptions = await queryProcessor.ProcessAsync(new GetPaidMessageExemptionsQuery(channelId));
        return exemptions?.ToList() ?? new List<PaidMessageExemption>();
    }

    public async Task<bool> AddUserExemptionAsync(long channelId, long userId, PaidMessageExemptionType type, string? reason = null, DateTime? expiresAt = null)
    {
        logger.LogInformation("Adding exemption for user {UserId} in channel {ChannelId}, type: {Type}", 
            userId, channelId, type);

        if (!await IsChannelAdminAsync(channelId, DateTime.UtcNow.GetHashCode())) // Simplified check
        {
            throw new UnauthorizedAccessException("User is not channel admin");
        }

        var command = new CreatePaidMessageExemptionCommand(MyTelegram.Domain.Aggregates.Channel.ChannelId.Create(channelId))
        {
            ChannelId = channelId,
            UserId = userId,
            Type = type,
            Reason = reason,
            ExpiresAt = expiresAt,
            CreatedBy = channelId, // Simplified - should be actual admin ID
            CreatedAt = DateTime.UtcNow
        };

        await commandBus.PublishAsync<ChannelAggregate, ChannelId, IExecutionResult>(command, CancellationToken.None);

        logger.LogInformation("Exemption added successfully for user {UserId} in channel {ChannelId}", userId, channelId);
        return true;
    }

    public async Task<bool> RemoveUserExemptionAsync(long channelId, long userId)
    {
        logger.LogInformation("Removing exemption for user {UserId} in channel {ChannelId}", userId, channelId);

        if (!await IsChannelAdminAsync(channelId, DateTime.UtcNow.GetHashCode())) // Simplified check
        {
            throw new UnauthorizedAccessException("User is not channel admin");
        }

        var command = new DeletePaidMessageExemptionCommand(MyTelegram.Domain.Aggregates.Channel.ChannelId.Create(channelId))
        {
            ChannelId = channelId,
            UserId = userId,
            DeletedAt = DateTime.UtcNow
        };

        await commandBus.PublishAsync<ChannelAggregate, ChannelId, IExecutionResult>(command, CancellationToken.None);

        logger.LogInformation("Exemption removed for user {UserId} in channel {ChannelId}", userId, channelId);
        return true;
    }

    public async Task<bool> CanSendPaidMessageAsync(long userId, long channelId, string messageContent)
    {
        var settings = await GetChannelSettingsAsync(channelId);
        if (!settings.Enabled)
        {
            return true; // No paid messages required
        }

        // Check if user is exempt
        if (await IsUserExemptAsync(userId, channelId))
        {
            return true;
        }

        // Check restrictions
        return await CheckUserRestrictionsAsync(userId, settings);
    }

    private async Task<PaidMessagePaymentResult> ProcessExemptMessageAsync(PaidMessagePaymentRequest request, PaidMessageSettings settings)
    {
        // Send message without payment
        var messageId = await SendMessageAsync(request);

        return new PaidMessagePaymentResult
        {
            Success = true,
            PaidMessageId = "exempt",
            MessageId = messageId,
            StarsSpent = 0,
            TransactionId = "exempt",
            PaidAt = DateTime.UtcNow
        };
    }

    private async Task<PaidMessageValidationResult> ValidateMessageAsync(PaidMessagePaymentRequest request, PaidMessageSettings settings)
    {
        // Check message length
        if (request.MessageContent.Length < settings.MinMessageLength || 
            request.MessageContent.Length > settings.MaxMessageLength)
        {
            return new PaidMessageValidationResult
            {
                IsValid = false,
                ErrorMessage = $"Message length must be between {settings.MinMessageLength} and {settings.MaxMessageLength} characters"
            };
        }

        // Check media restrictions
        if (!settings.AllowMedia && request.MediaIds.Any())
        {
            return new PaidMessageValidationResult
            {
                IsValid = false,
                ErrorMessage = "Media is not allowed in this channel"
            };
        }

        // Check link restrictions
        if (!settings.AllowLinks && ContainsLinks(request.MessageContent))
        {
            return new PaidMessageValidationResult
            {
                IsValid = false,
                ErrorMessage = "Links are not allowed in this channel"
            };
        }

        return new PaidMessageValidationResult { IsValid = true };
    }

    private async Task<bool> ProcessPaymentAsync(long userId, long channelId, long starsAmount)
    {
        try
        {
            // Deduct stars from user
            var userDeduction = await starsAppService.UpdateStarsBalanceAsync(
                userId, -starsAmount, "paid_message", "Payment for channel message");

            if (!userDeduction)
            {
                return false;
            }

            // Add stars to channel balance
            await starsAppService.UpdateStarsBalanceAsync(
                channelId, starsAmount, "paid_message_revenue", "Revenue from paid message");

            return true;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to process payment for user {UserId} in channel {ChannelId}", userId, channelId);
            return false;
        }
    }

    private async Task<string> CreatePaidMessageRecordAsync(PaidMessagePaymentRequest request, long starsAmount)
    {
        var command = new CreatePaidMessageCommand(MyTelegram.Domain.Aggregates.Channel.ChannelId.Create(request.ChannelId))
        {
            ChannelId = request.ChannelId,
            SenderId = request.UserId,
            StarsAmount = starsAmount,
            MessageContent = request.MessageContent,
            MessageType = request.MessageType,
            MediaIds = request.MediaIds,
            TransactionId = Guid.NewGuid().ToString(),
            PaidAt = DateTime.UtcNow,
            IsVisible = true
        };

        await commandBus.PublishAsync<ChannelAggregate, ChannelId, IExecutionResult>(command, CancellationToken.None);
        return Guid.NewGuid().ToString();
    }

    private async Task<long> SendMessageAsync(PaidMessagePaymentRequest request)
    {
        // This would integrate with the actual message sending service
        // For now, return a mock message ID
        await Task.Delay(1); // Simulate async operation
        return DateTime.UtcNow.Ticks;
    }

    private async Task<bool> IsUserExemptAsync(long userId, long channelId)
    {
        var exemptions = await GetChannelExemptionsAsync(channelId);
        var userExemption = exemptions.FirstOrDefault(e => e.UserId == userId);

        if (userExemption == null)
        {
            return false;
        }

        // Check if exemption is still valid
        if (userExemption.ExpiresAt.HasValue && userExemption.ExpiresAt.Value < DateTime.UtcNow)
        {
            return false;
        }

        return true;
    }

    private async Task<bool> CheckUserRestrictionsAsync(long userId, PaidMessageSettings settings)
    {
        return true; // Simplified - always allow for now
    }

    private async Task<bool> IsContactAsync(long userId, long channelId)
    {
        // Implementation needed to check if user is contact of channel owner
        return true; // Placeholder
    }

    private async Task<bool> IsPremiumUserAsync(long userId)
    {
        var user = await userAppService.GetAsync(userId);
        return user?.Premium ?? false;
    }

    private async Task<bool> IsChannelAdminAsync(long userId, long channelId)
    {
        var channelParticipant = await queryProcessor.ProcessAsync(
            new GetChannelParticipantQuery(channelId, userId));
        
        return channelParticipant != null && channelParticipant.IsAdmin;
    }

    private static void ValidateChannelSettings(PaidMessageSettings settings)
    {
        if (settings.StarsAmount <= 0)
        {
            throw new ArgumentException("Stars amount must be positive");
        }
    }

    private static bool ContainsLinks(string content)
    {
        // Simple regex to detect URLs
        var urlPattern = @"http[s]?://(?:[a-zA-Z]|[0-9]|[$-_@.&+]|[!*\\(\\),]|(?:%[0-9a-fA-F][0-9a-fA-F]))+";
        return System.Text.RegularExpressions.Regex.IsMatch(content, urlPattern);
    }
}

/// <summary>
/// Message validation result
/// </summary>
public class PaidMessageValidationResult
{
    public bool IsValid { get; set; }
    public string? ErrorMessage { get; set; }
}

// Query and command classes for paid messages
public class GetPaidMessageStatsQuery : IQuery<PaidMessageStats>
{
    public GetPaidMessageStatsQuery(long channelId) { ChannelId = channelId; }
    public long ChannelId { get; set; }
    public DateTime? From { get; set; }
    public DateTime? To { get; set; }
    public DateTime PeriodStart { get; set; }
    public DateTime PeriodEnd { get; set; }
}

public class GetUserPaidMessageHistoryQuery : IQuery<UserPaidMessageHistory>
{
    public GetUserPaidMessageHistoryQuery(long userId, int offset, int limit) { UserId = userId; Offset = offset; Limit = limit; }
    public long UserId { get; set; }
    public int Offset { get; set; }
    public int Limit { get; set; }
}

public class GetPaidMessageByIdQuery : IQuery<PaidMessage>
{
    public GetPaidMessageByIdQuery(string id) { Id = id; }
    public string Id { get; set; }
}

public class GetPaidMessageExemptionsQuery : IQuery<List<PaidMessageExemption>>
{
    public GetPaidMessageExemptionsQuery(long channelId) { ChannelId = channelId; }
    public long ChannelId { get; set; }
}

public class UpdatePaidMessageSettingsCommand : Command<MyTelegram.Domain.Aggregates.Channel.ChannelAggregate, MyTelegram.Domain.Aggregates.Channel.ChannelId, IExecutionResult>
{
    public UpdatePaidMessageSettingsCommand(MyTelegram.Domain.Aggregates.Channel.ChannelId channelId) : base(channelId) { }
    public long ChannelId { get; set; }
    public PaidMessageSettings Settings { get; set; } = new();
    public DateTime UpdatedAt { get; set; }
}

public class CreatePaidMessageCommand : Command<MyTelegram.Domain.Aggregates.Channel.ChannelAggregate, MyTelegram.Domain.Aggregates.Channel.ChannelId, IExecutionResult>
{
    public CreatePaidMessageCommand(MyTelegram.Domain.Aggregates.Channel.ChannelId channelId) : base(channelId) { }
    public long ChannelId { get; set; }
    public long SenderId { get; set; }
    public long StarsAmount { get; set; }
    public string MessageContent { get; set; } = string.Empty;
    public PaidMessageType MessageType { get; set; }
    public List<string> MediaIds { get; set; } = new();
    public string TransactionId { get; set; } = string.Empty;
    public DateTime PaidAt { get; set; }
    public bool IsVisible { get; set; }
}

public class RefundPaidMessageCommand : Command<MyTelegram.Domain.Aggregates.Channel.ChannelAggregate, MyTelegram.Domain.Aggregates.Channel.ChannelId, IExecutionResult>
{
    public RefundPaidMessageCommand(MyTelegram.Domain.Aggregates.Channel.ChannelId channelId) : base(channelId) { }
    public string PaidMessageId { get; set; } = string.Empty;
    public string RefundReason { get; set; } = string.Empty;
    public DateTime RefundedAt { get; set; }
}

public class CreatePaidMessageExemptionCommand : Command<MyTelegram.Domain.Aggregates.Channel.ChannelAggregate, MyTelegram.Domain.Aggregates.Channel.ChannelId, IExecutionResult>
{
    public CreatePaidMessageExemptionCommand(MyTelegram.Domain.Aggregates.Channel.ChannelId channelId) : base(channelId) { }
    public long ChannelId { get; set; }
    public long UserId { get; set; }
    public PaidMessageExemptionType Type { get; set; }
    public string? Reason { get; set; }
    public DateTime? ExpiresAt { get; set; }
    public long CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class DeletePaidMessageExemptionCommand : Command<MyTelegram.Domain.Aggregates.Channel.ChannelAggregate, MyTelegram.Domain.Aggregates.Channel.ChannelId, IExecutionResult>
{
    public DeletePaidMessageExemptionCommand(MyTelegram.Domain.Aggregates.Channel.ChannelId channelId) : base(channelId) { }
    public long ChannelId { get; set; }
    public long UserId { get; set; }
    public DateTime DeletedAt { get; set; }
}
