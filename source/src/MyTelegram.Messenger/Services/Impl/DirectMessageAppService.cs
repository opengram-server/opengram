using Microsoft.Extensions.Logging;
using MyTelegram.Domain.Shared.DirectMessages;
using MyTelegram.Domain.Shared.Business;
using MyTelegram.Domain.Aggregates.Channel;
using EventFlow.Aggregates.ExecutionResults;
using EventFlow.Commands;

namespace MyTelegram.Messenger.Services.Impl;

/// <summary>
/// Interface for managing direct message functionality
/// </summary>
public interface IDirectMessageAppService
{
    Task<DirectMessagesTopic> CreateTopicAsync(CreateDirectMessageTopicRequest request);
    Task<DirectMessagesTopic?> GetTopicAsync(string topicId);
    Task<List<DirectMessagesTopic>> GetChannelTopicsAsync(long channelId, int offset = 0, int limit = 50);
    Task<List<DirectMessagesTopic>> GetUserTopicsAsync(long userId, int offset = 0, int limit = 50);
    Task<CreateDirectMessageResult> SendMessageAsync(CreateDirectMessageRequest request);
    Task<DirectMessage?> GetMessageAsync(string messageId);
    Task<List<DirectMessage>> GetTopicMessagesAsync(string topicId, int offset = 0, int limit = 50);
    Task<bool> UpdateDirectMessageSettingsAsync(long channelId, DirectMessageSettings settings, long updatedBy);
    Task<bool> UpdatePricingAsync(long channelId, DirectMessagePricing pricing, long updatedBy);
    Task<DirectMessageStatistics> GetStatisticsAsync(long channelId, DateTime? from = null, DateTime? to = null);
    Task<bool> PerformModerationActionAsync(long channelId, long moderatorId, DirectMessageModerationType action, long targetUserId, string? messageId = null, string? reason = null);
    Task<bool> MarkMessageAsReadAsync(string messageId, long userId);
    Task<bool> DeleteMessageAsync(string messageId, long deletedBy);
    Task<bool> CloseTopicAsync(string topicId, long closedBy);
    Task<bool> ArchiveTopicAsync(string topicId, long archivedBy);
}

/// <summary>
/// Service for managing direct message functionality
/// </summary>
public sealed class DirectMessageAppService(
    ILogger<DirectMessageAppService> logger,
    IQueryProcessor queryProcessor,
    ICommandBus commandBus,
    IUserAppService userAppService,
    IStarsAppService starsAppService) : IDirectMessageAppService
{
    public async Task<DirectMessagesTopic> CreateTopicAsync(CreateDirectMessageTopicRequest request)
    {
        logger.LogInformation("Creating direct message topic for channel {ChannelId} by user {UserId}", 
            request.ChannelId, request.UserId);

        // Check if channel allows direct messages
        var channelSettings = await GetChannelDirectMessageSettingsAsync(request.ChannelId);
        if (channelSettings == null || !channelSettings.AllowDirectMessages)
        {
            throw new InvalidOperationException("Channel does not allow direct messages");
        }

        // Check if topic already exists
        var existingTopic = await GetExistingTopicAsync(request.ChannelId, request.UserId);
        if (existingTopic != null)
        {
            throw new InvalidOperationException("Direct message topic already exists for this user");
        }

        var topicId = Guid.NewGuid().ToString();
        var localSettings = new DirectMessageSettings();

        var command = new CreateDirectMessageTopicCommand(
            new ChannelId(request.ChannelId.ToString()),
            topicId,
            request.UserId,
            DateTime.UtcNow,
            DirectMessageTopicStatus.Active,
            true,
            localSettings);

        await commandBus.PublishAsync<ChannelAggregate, ChannelId, IExecutionResult>(command, CancellationToken.None);

        var topic = new DirectMessagesTopic
        {
            Id = topicId,
            ChannelId = request.ChannelId,
            UserId = request.UserId,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            Status = DirectMessageTopicStatus.Active,
            IsActive = true,
            Settings = localSettings
        };

        // Send initial message if provided
        if (!string.IsNullOrEmpty(request.InitialMessage))
        {
            var messageRequest = new CreateDirectMessageRequest
            {
                TopicId = topicId,
                SenderId = request.UserId,
                ChannelId = request.ChannelId,
                Content = request.InitialMessage,
                Type = DirectMessageType.Text
            };

            await SendMessageAsync(messageRequest);
        }

        logger.LogInformation("Direct message topic created successfully for channel {ChannelId} and user {UserId}", 
            request.ChannelId, request.UserId);

        return topic;
    }

    public async Task<DirectMessagesTopic?> GetTopicAsync(string topicId)
    {
        var topic = await queryProcessor.ProcessAsync(new GetDirectMessageTopicQuery(topicId));
        return topic;
    }

    public async Task<List<DirectMessagesTopic>> GetChannelTopicsAsync(long channelId, int offset = 0, int limit = 50)
    {
        var topics = await queryProcessor.ProcessAsync(
            new GetChannelDirectMessageTopicsQuery(channelId, offset, limit));

        return topics?.ToList() ?? new List<DirectMessagesTopic>();
    }

    public async Task<List<DirectMessagesTopic>> GetUserTopicsAsync(long userId, int offset = 0, int limit = 50)
    {
        var topics = await queryProcessor.ProcessAsync(
            new GetUserDirectMessageTopicsQuery(userId, offset, limit));

        return topics?.ToList() ?? new List<DirectMessagesTopic>();
    }

    public async Task<CreateDirectMessageResult> SendMessageAsync(CreateDirectMessageRequest request)
    {
        logger.LogInformation("Sending direct message to topic {TopicId} by user {UserId}", 
            request.TopicId, request.SenderId);

        var topic = await GetTopicAsync(request.TopicId);
        if (topic == null || !topic.IsActive)
        {
            return new CreateDirectMessageResult
            {
                Success = false,
                ErrorMessage = "Topic not found or inactive"
            };
        }

        // Validate message
        var validationResult = await ValidateMessageAsync(request, new MyTelegram.Domain.Shared.DirectMessages.DirectMessageSettings());
        if (!validationResult.IsValid)
        {
            return new CreateDirectMessageResult
            {
                Success = false,
                ErrorMessage = validationResult.ErrorMessage
            };
        }

        // Process payment if required
        long starsCharged = 0;
        if (validationResult.RequiresPayment)
        {
            var paymentResult = await ProcessPaymentAsync(request.SenderId, topic.ChannelId, validationResult.RequiredStars);
            if (!paymentResult.Success)
            {
                return new CreateDirectMessageResult
                {
                    Success = false,
                    ErrorMessage = "Payment failed",
                    RequiresPayment = true,
                    PaymentUrl = paymentResult.PaymentUrl
                };
            }
            starsCharged = validationResult.RequiredStars;
        }

        var messageId = Guid.NewGuid().ToString();
        var receiverId = topic.UserId == request.SenderId ? topic.ChannelId : topic.UserId;

        var command = new CreateDirectMessageCommand(
            new ChannelId(request.ChannelId.ToString()),
            messageId,
            request.TopicId,
            request.SenderId,
            receiverId,
            request.Content,
            request.Type,
            request.MediaAttachments,
            request.ReplyToMessageId,
            starsCharged,
            starsCharged > 0,
            DirectMessageStatus.Sent);

        await commandBus.PublishAsync<ChannelAggregate, ChannelId, IExecutionResult>(command, CancellationToken.None);

        var message = new DirectMessage
        {
            Id = messageId,
            TopicId = request.TopicId,
            ChannelId = request.ChannelId,
            SenderId = request.SenderId,
            ReceiverId = receiverId,
            Content = request.Content,
            Type = request.Type,
            MediaAttachments = request.MediaAttachments,
            ReplyToMessageId = request.ReplyToMessageId,
            SentAt = DateTime.UtcNow,
            StarsCost = starsCharged,
            IsPaid = starsCharged > 0,
            Status = DirectMessageStatus.Sent
        };

        // Update topic activity
        await UpdateTopicActivityAsync(request.TopicId, long.Parse(messageId), request.SenderId, request.Content, request.ChannelId);

        logger.LogInformation("Direct message sent successfully to topic {TopicId}", request.TopicId);

        return new CreateDirectMessageResult
        {
            Success = true,
            Message = message,
            StarsCharged = starsCharged,
            RequiresPayment = false
        };
    }

    public async Task<DirectMessage?> GetMessageAsync(string messageId)
    {
        var message = await queryProcessor.ProcessAsync(new GetDirectMessageQuery(messageId));
        return message;
    }

    public async Task<List<DirectMessage>> GetTopicMessagesAsync(string topicId, int offset = 0, int limit = 50)
    {
        var messages = await queryProcessor.ProcessAsync(
            new GetDirectMessagesQuery(topicId, offset, limit));

        return messages?.ToList() ?? new List<DirectMessage>();
    }

    public async Task<bool> UpdateDirectMessageSettingsAsync(long channelId, DirectMessageSettings settings, long updatedBy)
    {
        logger.LogInformation("Updating direct message settings for channel {ChannelId} by user {UserId}", 
            channelId, updatedBy);

        if (!await IsChannelAdminAsync(updatedBy, channelId))
        {
            throw new UnauthorizedAccessException("User is not channel admin");
        }

        var command = new UpdateDirectMessageSettingsCommand(
            new MyTelegram.Domain.Aggregates.Channel.ChannelId(channelId.ToString()),
            updatedBy,
            DateTime.UtcNow,
            new DirectMessageSettings());

        await commandBus.PublishAsync<ChannelAggregate, ChannelId, IExecutionResult>(command, CancellationToken.None);

        logger.LogInformation("Direct message settings updated for channel {ChannelId}", channelId);
        return true;
    }

    public async Task<bool> UpdatePricingAsync(long channelId, DirectMessagePricing pricing, long updatedBy)
    {
        logger.LogInformation("Updating direct message pricing for channel {ChannelId} by user {UserId}", 
            channelId, updatedBy);

        if (!await IsChannelAdminAsync(updatedBy, channelId))
        {
            throw new UnauthorizedAccessException("User is not channel admin");
        }

        var command = new UpdateDirectMessagePricingCommand(
            new MyTelegram.Domain.Aggregates.Channel.ChannelId(channelId.ToString()),
            updatedBy,
            DateTime.UtcNow,
            new DirectMessagePricing { BasePrice = pricing.BasePrice });

        await commandBus.PublishAsync<ChannelAggregate, ChannelId, IExecutionResult>(command, CancellationToken.None);

        // Send service message about price change
        await SendPriceChangeNotificationAsync(channelId, pricing.BasePrice, updatedBy);

        logger.LogInformation("Direct message pricing updated for channel {ChannelId}", channelId);
        return true;
    }

    public async Task<DirectMessageStatistics> GetStatisticsAsync(long channelId, DateTime? from = null, DateTime? to = null)
    {
        logger.LogInformation("Getting direct message statistics for channel {ChannelId}", channelId);

        var query = new GetDirectMessageStatisticsQuery(channelId)
        {
            From = from ?? DateTime.UtcNow.AddDays(-30),
            To = to ?? DateTime.UtcNow
        };

        var stats = await queryProcessor.ProcessAsync(query);
        
        return stats ?? new DirectMessageStatistics
        {
            From = query.From ?? DateTime.UtcNow.AddMonths(-1),
            To = query.To ?? DateTime.UtcNow
        };
    }

    public async Task<bool> PerformModerationActionAsync(long channelId, long moderatorId, DirectMessageModerationType action, long targetUserId, string? messageId = null, string? reason = null)
    {
        logger.LogInformation("Performing moderation action {Action} in channel {ChannelId} by moderator {ModeratorId}", 
            action, channelId, moderatorId);

        if (!await IsChannelAdminAsync(moderatorId, channelId))
        {
            throw new UnauthorizedAccessException("User is not channel admin");
        }

        var command = new PerformDirectMessageModerationCommand(
            new MyTelegram.Domain.Aggregates.Channel.ChannelId(channelId.ToString()))
        {
            ChannelId = channelId,
            ModeratorId = moderatorId,
            Action = action,
            TargetUserId = targetUserId,
            MessageId = messageId,
            Reason = reason,
            PerformedAt = DateTime.UtcNow
        };

        await commandBus.PublishAsync<ChannelAggregate, ChannelId, IExecutionResult>(command, CancellationToken.None);

        logger.LogInformation("Moderation action {Action} performed successfully in channel {ChannelId}", action, channelId);
        return true;
    }

    public async Task<bool> MarkMessageAsReadAsync(string messageId, long userId)
    {
        logger.LogInformation("Marking message {MessageId} as read by user {UserId}", messageId, userId);

        var message = await GetMessageAsync(messageId);
        if (message == null || message.ReceiverId != userId)
        {
            return false;
        }

        var command = new MarkDirectMessageAsReadCommand(
            new MyTelegram.Domain.Aggregates.Channel.ChannelId(message.ChannelId.ToString()))
        {
            MessageId = messageId,
            UserId = userId,
            ReadAt = DateTime.UtcNow
        };

        await commandBus.PublishAsync<ChannelAggregate, ChannelId, IExecutionResult>(command, CancellationToken.None);

        logger.LogInformation("Message {MessageId} marked as read successfully", messageId);
        return true;
    }

    public async Task<bool> DeleteMessageAsync(string messageId, long deletedBy)
    {
        logger.LogInformation("Deleting message {MessageId} by user {UserId}", messageId, deletedBy);

        var message = await GetMessageAsync(messageId);
        if (message == null)
        {
            return false;
        }

        // Check if user can delete this message
        if (!await CanDeleteMessageAsync(deletedBy, message))
        {
            throw new UnauthorizedAccessException("User doesn't have permission to delete this message");
        }

        var command = new DeleteDirectMessageCommand(
            new MyTelegram.Domain.Aggregates.Channel.ChannelId(message.ChannelId.ToString()))
        {
            MessageId = messageId,
            DeletedBy = deletedBy,
            DeletedAt = DateTime.UtcNow
        };

        await commandBus.PublishAsync<ChannelAggregate, ChannelId, IExecutionResult>(command, CancellationToken.None);

        logger.LogInformation("Message {MessageId} deleted successfully", messageId);
        return true;
    }

    public async Task<bool> CloseTopicAsync(string topicId, long closedBy)
    {
        logger.LogInformation("Closing topic {TopicId} by user {UserId}", topicId, closedBy);

        var topic = await GetTopicAsync(topicId);
        if (topic == null)
        {
            return false;
        }

        if (!await CanCloseTopicAsync(closedBy, topic))
        {
            throw new UnauthorizedAccessException("User doesn't have permission to close this topic");
        }

        var command = new CloseDirectMessageTopicCommand(
            new MyTelegram.Domain.Aggregates.Channel.ChannelId(topic.ChannelId.ToString()))
        {
            TopicId = topicId,
            ClosedBy = closedBy,
            ClosedAt = DateTime.UtcNow
        };

        await commandBus.PublishAsync<ChannelAggregate, ChannelId, IExecutionResult>(command, CancellationToken.None);

        logger.LogInformation("Topic {TopicId} closed successfully", topicId);
        return true;
    }

    public async Task<bool> ArchiveTopicAsync(string topicId, long archivedBy)
    {
        logger.LogInformation("Archiving topic {TopicId} by user {UserId}", topicId, archivedBy);

        var topic = await GetTopicAsync(topicId);
        if (topic == null)
        {
            return false;
        }

        if (!await CanArchiveTopicAsync(archivedBy, topic))
        {
            throw new UnauthorizedAccessException("User doesn't have permission to archive this topic");
        }

        var command = new ArchiveDirectMessageTopicCommand(
            new MyTelegram.Domain.Aggregates.Channel.ChannelId(topic.ChannelId.ToString()))
        {
            TopicId = topicId,
            ArchivedBy = archivedBy,
            ArchivedAt = DateTime.UtcNow
        };

        await commandBus.PublishAsync<ChannelAggregate, ChannelId, IExecutionResult>(command, CancellationToken.None);

        logger.LogInformation("Topic {TopicId} archived successfully", topicId);
        return true;
    }

    private async Task<MyTelegram.Domain.Shared.DirectMessages.DirectMessageSettings?> GetChannelDirectMessageSettingsAsync(long channelId)
    {
        // In a real implementation, this would retrieve from database
        return await Task.FromResult(new MyTelegram.Domain.Shared.DirectMessages.DirectMessageSettings
        {
            AllowDirectMessages = true,
            PricePerMessage = 100, // 100 stars per message
            RequireVerification = false,
            AllowMedia = true,
            AllowLinks = false,
            MaxMessageLength = 4096,
            RequirePayment = true
        });
    }

    private async Task<DirectMessagesTopic?> GetExistingTopicAsync(long channelId, long userId)
    {
        var topic = await queryProcessor.ProcessAsync(new GetExistingDirectMessageTopicQuery(channelId, userId));
        return topic;
    }

    private async Task<DirectMessageValidationResult> ValidateMessageAsync(CreateDirectMessageRequest request, MyTelegram.Domain.Shared.DirectMessages.DirectMessageSettings settings)
    {
        var result = new DirectMessageValidationResult { IsValid = true };

        // Check message length
        if (string.IsNullOrWhiteSpace(request.Content) || request.Content.Length > settings.MaxMessageLength)
        {
            result.IsValid = false;
            result.ErrorMessage = $"Message length must be between 1 and {settings.MaxMessageLength} characters";
            return result;
        }

        // Check content restrictions
        if (!settings.AllowLinks && ContainsLinks(request.Content))
        {
            result.IsValid = false;
            result.ErrorMessage = "Links are not allowed in direct messages";
            return result;
        }

        // Check media restrictions
        if (request.Type != DirectMessageType.Text && !settings.AllowMedia)
        {
            result.IsValid = false;
            result.ErrorMessage = "Media is not allowed in direct messages";
            return result;
        }

        // Check payment requirements
        if (settings.RequirePayment && settings.PricePerMessage > 0)
        {
            result.RequiresPayment = true;
            result.RequiredStars = settings.PricePerMessage;
        }

        return result;
    }

    private async Task<PaymentResult> ProcessPaymentAsync(long userId, long channelId, long starsAmount)
    {
        try
        {
            // Check user balance
            var starsStatus = await starsAppService.GetStarsStatusAsync(userId);
            var balance = starsStatus.Balance;
            if (balance < starsAmount)
            {
                return new PaymentResult
                {
                    Success = false,
                    PaymentUrl = $"/stars/topup?amount={starsAmount}"
                };
            }

            // Process payment
            var paymentResult = await starsAppService.UpdateStarsBalanceAsync(userId, -starsAmount, "direct_message_payment", "Direct message payment");
            await starsAppService.UpdateStarsBalanceAsync(channelId, starsAmount, "direct_message_receive", "Direct message payment received");
            
            return new PaymentResult { Success = paymentResult };
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error processing payment for direct message");
            return new PaymentResult
            {
                Success = false,
                PaymentUrl = "/stars/topup"
            };
        }
    }

    private async Task UpdateTopicActivityAsync(string topicId, long messageId, long senderId, string content, long channelId)
    {
        var command = new UpdateDirectMessageTopicActivityCommand(
            new MyTelegram.Domain.Aggregates.Channel.ChannelId(channelId.ToString()),
            topicId,
            messageId,
            senderId,
            DateTime.UtcNow,
            content,
            DateTime.UtcNow);

        await commandBus.PublishAsync<ChannelAggregate, ChannelId, IExecutionResult>(command, CancellationToken.None);
    }

    private async Task SendPriceChangeNotificationAsync(long channelId, long newPrice, long changedBy)
    {
        var command = new UpdateDirectMessagePricingCommand(
            new MyTelegram.Domain.Aggregates.Channel.ChannelId(channelId.ToString()),
            changedBy,
            DateTime.UtcNow,
            new DirectMessagePricing { BasePrice = newPrice });

        await commandBus.PublishAsync<ChannelAggregate, ChannelId, IExecutionResult>(command, CancellationToken.None);
    }

    private async Task<bool> IsChannelAdminAsync(long userId, long channelId)
    {
        // In a real implementation, this would check channel participant permissions
        return await Task.FromResult(true); // Placeholder
    }

    private async Task<bool> CanDeleteMessageAsync(long userId, DirectMessage message)
    {
        // Users can delete their own messages
        if (message.SenderId == userId)
        {
            return true;
        }

        // Channel admins can delete any message
        return await IsChannelAdminAsync(userId, message.ChannelId);
    }

    private async Task<bool> CanCloseTopicAsync(long userId, DirectMessagesTopic topic)
    {
        // Users can close their own topics
        if (topic.UserId == userId)
        {
            return true;
        }

        // Channel admins can close any topic
        return await IsChannelAdminAsync(userId, topic.ChannelId);
    }

    private async Task<bool> CanArchiveTopicAsync(long userId, DirectMessagesTopic topic)
    {
        // Users can archive their own topics
        if (topic.UserId == userId)
        {
            return true;
        }

        // Channel admins can archive any topic
        return await IsChannelAdminAsync(userId, topic.ChannelId);
    }

    private static bool ContainsLinks(string content)
    {
        var urlPattern = @"http[s]?://(?:[a-zA-Z]|[0-9]|[$-_@.&+]|[!*\\(\\),]|(?:%[0-9a-fA-F][0-9a-fA-F]))+";
        return System.Text.RegularExpressions.Regex.IsMatch(content, urlPattern);
    }
}

// Helper classes
public class PaymentResult
{
    public bool Success { get; set; }
    public string PaymentUrl { get; set; } = string.Empty;
}

public class GetDirectMessageQuery : IQuery<DirectMessage>
{
    public string MessageId { get; set; }
    
    public GetDirectMessageQuery(string messageId)
    {
        MessageId = messageId;
    }
}

public class GetDirectMessagesQuery : IQuery<List<DirectMessage>>
{
    public string TopicId { get; set; }
    public int Offset { get; set; }
    public int Limit { get; set; }
    
    public GetDirectMessagesQuery(string topicId, int offset, int limit)
    {
        TopicId = topicId;
        Offset = offset;
        Limit = limit;
    }
}

public class CreateDirectMessageTopicCommand : Command<ChannelAggregate, ChannelId>
{
    public CreateDirectMessageTopicCommand(ChannelId channelId, string topicId, long userId, DateTime createdAt, DirectMessageTopicStatus status, bool isActive, DirectMessageSettings settings) 
        : base(channelId) 
    {
        TopicId = topicId;
        UserId = userId;
        CreatedAt = createdAt;
        Status = status;
        IsActive = isActive;
        Settings = settings;
    }
    public string TopicId { get; set; } = string.Empty;
    public long UserId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DirectMessageTopicStatus Status { get; set; }
    public bool IsActive { get; set; }
    public DirectMessageSettings Settings { get; set; } = new();
}

public class CreateDirectMessageCommand : Command<ChannelAggregate, ChannelId>
{
    public CreateDirectMessageCommand(ChannelId channelId, string messageId, string topicId, long senderId, long receiverId, string content, DirectMessageType type, List<DirectMessageMediaAttachment> mediaAttachments, long replyToMessageId, long starsCost, bool isPaid, DirectMessageStatus status) 
        : base(channelId) 
    {
        MessageId = messageId;
        TopicId = topicId;
        SenderId = senderId;
        ReceiverId = receiverId;
        Content = content;
        Type = type;
        MediaAttachments = mediaAttachments;
        ReplyToMessageId = replyToMessageId;
        StarsCost = starsCost;
        IsPaid = isPaid;
        Status = status;
    }
    public string MessageId { get; set; } = string.Empty;
    public string TopicId { get; set; } = string.Empty;
    public long SenderId { get; set; }
    public long ReceiverId { get; set; }
    public string Content { get; set; } = string.Empty;
    public DirectMessageType Type { get; set; }
    public List<DirectMessageMediaAttachment> MediaAttachments { get; set; } = new();
    public long ReplyToMessageId { get; set; }
    public long StarsCost { get; set; }
    public bool IsPaid { get; set; }
    public DirectMessageStatus Status { get; set; }
}

public class UpdateDirectMessageSettingsCommand : Command<ChannelAggregate, ChannelId>
{
    public UpdateDirectMessageSettingsCommand(ChannelId channelId, long updatedBy, DateTime updatedAt, DirectMessageSettings settings) 
        : base(channelId) 
    {
        UpdatedBy = updatedBy;
        UpdatedAt = updatedAt;
        Settings = settings;
    }
    public long UpdatedBy { get; set; }
    public DateTime UpdatedAt { get; set; }
    public DirectMessageSettings Settings { get; set; } = new();
}

public class UpdateDirectMessagePricingCommand : Command<ChannelAggregate, ChannelId>
{
    public UpdateDirectMessagePricingCommand(ChannelId channelId, long updatedBy, DateTime updatedAt, DirectMessagePricing pricing) 
        : base(channelId) 
    {
        UpdatedBy = updatedBy;
        UpdatedAt = updatedAt;
        Pricing = pricing;
    }
    public long UpdatedBy { get; set; }
    public DateTime UpdatedAt { get; set; }
    public DirectMessagePricing Pricing { get; set; } = new();
}

public class UpdateDirectMessageTopicActivityCommand : Command<ChannelAggregate, ChannelId>
{
    public UpdateDirectMessageTopicActivityCommand(ChannelId channelId, string topicId, long lastMessageId, long lastSenderId, DateTime lastMessageDate, string lastMessagePreview, DateTime updatedAt) 
        : base(channelId) 
    {
        TopicId = topicId;
        LastMessageId = lastMessageId;
        LastSenderId = lastSenderId;
        LastMessageDate = lastMessageDate;
        LastMessagePreview = lastMessagePreview;
        UpdatedAt = updatedAt;
    }
    public string TopicId { get; set; } = string.Empty;
    public long LastMessageId { get; set; }
    public long LastSenderId { get; set; }
    public DateTime LastMessageDate { get; set; }
    public string LastMessagePreview { get; set; } = string.Empty;
    public DateTime UpdatedAt { get; set; }
}

public class MarkDirectMessageAsReadCommand : Command<ChannelAggregate, ChannelId>
{
    public MarkDirectMessageAsReadCommand(ChannelId channelId) : base(channelId) { }
    public string MessageId { get; set; } = string.Empty;
    public long UserId { get; set; }
    public DateTime ReadAt { get; set; }
}

public class DeleteDirectMessageCommand : Command<ChannelAggregate, ChannelId>
{
    public DeleteDirectMessageCommand(ChannelId channelId) : base(channelId) { }
    public string MessageId { get; set; } = string.Empty;
    public long DeletedBy { get; set; }
    public DateTime DeletedAt { get; set; }
}

public class CloseDirectMessageTopicCommand : Command<ChannelAggregate, ChannelId>
{
    public CloseDirectMessageTopicCommand(ChannelId channelId) : base(channelId) { }
    public string TopicId { get; set; } = string.Empty;
    public long ClosedBy { get; set; }
    public DateTime ClosedAt { get; set; }
}

public class ArchiveDirectMessageTopicCommand : Command<ChannelAggregate, ChannelId>
{
    public ArchiveDirectMessageTopicCommand(ChannelId channelId) : base(channelId) { }
    public string TopicId { get; set; } = string.Empty;
    public long ArchivedBy { get; set; }
    public DateTime ArchivedAt { get; set; }
}

public class PerformDirectMessageModerationCommand : Command<ChannelAggregate, ChannelId>
{
    public PerformDirectMessageModerationCommand(ChannelId channelId) : base(channelId) { }
    public long ChannelId { get; set; }
    public long ModeratorId { get; set; }
    public DirectMessageModerationType Action { get; set; }
    public long TargetUserId { get; set; }
    public string? MessageId { get; set; }
    public string? Reason { get; set; }
    public DateTime PerformedAt { get; set; }
}

public class SendDirectMessagePriceChangeCommand : Command<ChannelAggregate, ChannelId>
{
    public SendDirectMessagePriceChangeCommand(ChannelId channelId) : base(channelId) { }
    public long ChannelId { get; set; }
    public long NewPrice { get; set; }
    public long ChangedBy { get; set; }
    public DateTime ChangedAt { get; set; }
}
