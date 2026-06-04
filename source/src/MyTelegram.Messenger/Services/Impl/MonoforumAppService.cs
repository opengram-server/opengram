using Microsoft.Extensions.Logging;
using MyTelegram.Domain.Shared.Business;
using MyTelegram.Domain.Aggregates.Channel;
using EventFlow.Aggregates.ExecutionResults;
using EventFlow.Commands;
using MyTelegram.Messenger.Services;
using MyTelegram.Domain.Shared.Forums;
using MyTelegram;
using DomainMonoforum = MyTelegram.Domain.Shared.Forums.Monoforum;
using DomainMonoforumSettings = MyTelegram.Domain.Shared.Forums.MonoforumSettings;
using DomainMonoforumTopic = MyTelegram.Domain.Shared.Forums.MonoforumTopic;

namespace MyTelegram.Messenger.Services.Impl;

/// <summary>
/// Service for managing monoforum functionality
/// </summary>
public sealed class MonoforumAppService(
    ILogger<MonoforumAppService> logger,
    IQueryProcessor queryProcessor,
    ICommandBus commandBus,
    IUserAppService userAppService) : IMonoforumAppService
{
    public async Task<DomainMonoforum> CreateMonoforumAsync(CreateMonoforumRequest request)
    {
        logger.LogInformation("Creating monoforum for channel {ChannelId} by user {UserId}", 
            request.ChannelId, request.CreatorId);

        // Check if user is channel admin
        if (!await IsChannelAdminAsync(request.CreatorId, request.ChannelId))
        {
            throw new UnauthorizedAccessException("User is not channel admin");
        }

        // Check if monoforum already exists
        var existingMonoforum = await GetMonoforumAsync(request.ChannelId);
        if (existingMonoforum != null)
        {
            throw new InvalidOperationException("Monoforum already exists for this channel");
        }

        // Generate unique ID for the monoforum channel
        // Monoforums use a different ID range - we'll use a high range to distinguish them
        var monoforumChannelId = await GenerateMonoforumIdAsync(request.ChannelId);
        
        // Generate access hash for the monoforum
        var accessHash = GenerateAccessHash(monoforumChannelId);
        
        // Create the monoforum as a real Channel (supergroup with monoforum flag)
        var createChannelCommand = new MyTelegram.Domain.Commands.Channel.CreateChannelCommand(
            aggregateId: ChannelId.Create(monoforumChannelId),
            requestInfo: MyTelegram.RequestInfo.Empty with { UserId = request.CreatorId, ReqMsgId = Random.Shared.NextInt64() },
            channelId: monoforumChannelId,
            creatorId: request.CreatorId,
            title: request.Title ?? "Direct Messages",
            broadcast: false,  // broadcast = false (it's a supergroup)
            megaGroup: true,   // megaGroup = true (supergroup)
            about: request.Description ?? "Send direct messages to this channel",
            geoPoint: null,
            address: null,
            accessHash: accessHash,
            date: DateTime.UtcNow.ToTimestamp(),
            randomId: Random.Shared.NextInt64(),
            messageAction: new MyTelegram.Schema.TMessageActionChatCreate { Title = "Direct Messages", Users = new TVector<long>() },
            ttlPeriod: null,
            migratedFromChat: false,
            migrateFromChatId: null,
            migratedMaxId: null,
            photoId: null,
            autoCreateFromChat: false,
            ttlFromDefaultSetting: false,
            memberUserIds: null,
            botUserIds: null
        );

        try
        {
            await commandBus.PublishAsync<ChannelAggregate, ChannelId, IExecutionResult>(
                createChannelCommand, 
                CancellationToken.None);
        }
        catch (global::EventFlow.Exceptions.DomainError ex) when (ex.Message.Contains("is not new"))
        {
            // Channel already exists, ignore
            logger.LogWarning(ex, "Monoforum channel {MonoforumId} already exists, skipping creation", monoforumChannelId);
        }

        logger.LogInformation("Monoforum channel {MonoforumId} created for channel {ChannelId}", 
            monoforumChannelId, request.ChannelId);

        // Link the monoforum to the original channel (set monoforum.linked_monoforum_id = original channel ID)
        var linkMonoforumCommand = new LinkMonoforumCommand(
            ChannelId.Create(monoforumChannelId),
            MyTelegram.RequestInfo.Empty with { UserId = request.CreatorId, ReqMsgId = Random.Shared.NextInt64() },
            request.ChannelId,  // linkedMonoforumId = original channel
            true,  // isMonoforum = true
            false  // broadcastMessagesAllowed = false (only for original channel)
        );
        
        await commandBus.PublishAsync<ChannelAggregate, ChannelId, IExecutionResult>(
            linkMonoforumCommand,
            CancellationToken.None);

        // Link the original channel to the monoforum (set channel.linked_monoforum_id = monoforum ID)
        var linkChannelCommand = new LinkMonoforumCommand(
            ChannelId.Create(request.ChannelId),
            MyTelegram.RequestInfo.Empty with { UserId = request.CreatorId, ReqMsgId = Random.Shared.NextInt64() },
            monoforumChannelId,  // linkedMonoforumId = monoforum
            false,  // isMonoforum = false
            true  // broadcastMessagesAllowed = true (for original channel)
        );
        
        await commandBus.PublishAsync<ChannelAggregate, ChannelId, IExecutionResult>(
            linkChannelCommand,
            CancellationToken.None);

        logger.LogInformation("Monoforum {MonoforumId} and channel {ChannelId} linked successfully", 
            monoforumChannelId, request.ChannelId);

        var monoforum = new DomainMonoforum
        {
            Id = monoforumChannelId,
            ChannelId = request.ChannelId,
            CreatorId = request.CreatorId,
            IsMonoforum = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            Settings = request.Settings ?? new DomainMonoforumSettings
            {
                AllowPublicTopics = false
            }
        };

        logger.LogInformation("Monoforum created successfully for channel {ChannelId}", request.ChannelId);
        return monoforum;
    }

    private async Task<long> GenerateMonoforumIdAsync(long originalChannelId)
    {
        // Monoforums use a different ID range
        // We need to generate a unique channel ID for the monoforum
        // Use IIdGenerator to get the next channel ID
        var idGenerator = await Task.FromResult<IIdGenerator?>(null); // Will be injected
        
        // For now, use a deterministic approach based on the original channel ID
        // In production, this should use a proper ID generator service
        // Monoforum IDs are in a different range to distinguish them
        return 900000000000L + originalChannelId;
    }

    private static long GenerateAccessHash(long channelId)
    {
        // Generate a deterministic but unique access hash
        return BitConverter.ToInt64(System.Security.Cryptography.MD5.HashData(BitConverter.GetBytes(channelId)), 0);
    }

    public async Task<DomainMonoforum?> GetMonoforumAsync(long channelId)
    {
        var monoforum = await queryProcessor.ProcessAsync(new GetMonoforumQuery(channelId));
        if (monoforum == null) return null;

        return new DomainMonoforum
        {
            Id = long.TryParse(monoforum.Id, out var id) ? id : 0,
            ChannelId = monoforum.ChannelId,
            CreatorId = monoforum.CreatorId,
            IsMonoforum = monoforum.IsMonoforum,
            CreatedAt = monoforum.CreatedAt,
            UpdatedAt = monoforum.UpdatedAt,
            Settings = new DomainMonoforumSettings
            {
                AllowPublicTopics = monoforum.Settings.AllowPublicTopics,
                AutoModerateMessages = monoforum.Settings.AutoModerateMessages
            }
        };
    }

    public async Task<bool> UpdateMonoforumSettingsAsync(long channelId, DomainMonoforumSettings settings, long updatedBy)
    {
        logger.LogInformation("Updating monoforum settings for channel {ChannelId} by user {UserId}", 
            channelId, updatedBy);

        var monoforum = await GetMonoforumAsync(channelId);
        if (monoforum == null)
        {
            return false;
        }

        if (!await IsChannelAdminAsync(updatedBy, channelId))
        {
            throw new UnauthorizedAccessException("User is not channel admin");
        }

        var command = new UpdateMonoforumSettingsCommand(new ChannelId(channelId.ToString()))
        {
            ChannelId = channelId,
            Settings = new MonoforumSettings
            {
                AllowPublicTopics = settings.AllowPublicTopics,
                AutoModerateMessages = settings.AutoModerateMessages,
                RequireVerification = settings.ModerationRules?.RequireVerification ?? false,
                FilterProfanity = settings.ModerationRules?.FilterProfanity ?? true,
                FilterSpam = settings.ModerationRules?.FilterSpam ?? true,
                EnableSlowMode = settings.ModerationRules?.EnableSlowMode ?? false,
                RequireAccountAge = settings.ModerationRules?.RequireAccountAge ?? true,
                MinAccountAgeDays = settings.ModerationRules?.MinAccountAgeDays ?? 7,
                RequirePhoneNumber = settings.ModerationRules?.RequirePhoneNumber ?? true
            },
            UpdatedBy = updatedBy,
            UpdatedAt = DateTime.UtcNow
        };

        await commandBus.PublishAsync<ChannelAggregate, ChannelId, IExecutionResult>(command, CancellationToken.None);

        logger.LogInformation("Monoforum settings updated for channel {ChannelId}", channelId);
        return true;
    }

    public async Task<object> CreateTopicAsync(object requestObj)
    {
        var request = (MonoforumTopicRequest)requestObj;
        logger.LogInformation("Creating topic '{Title}' in monoforum channel {ChannelId} by user {UserId}", 
            request.Title, request.ChannelId, request.CreatorId);

        var monoforum = await GetMonoforumAsync(request.ChannelId);
        if (monoforum == null)
        {
            return new MonoforumTopicResult
            {
                Success = false,
                ErrorMessage = "Monoforum not found for this channel"
            };
        }

        // Validate topic request
        var validationResult = await ValidateTopicRequestAsync(request, monoforum);
        if (!validationResult.IsValid)
        {
            return new MonoforumTopicResult
            {
                Success = false,
                ErrorMessage = validationResult.ErrorMessage
            };
        }

        var topicId = Guid.NewGuid().ToString();
        var requiresApproval = false;

        var domainSettings = new MyTelegram.Domain.Shared.Forums.MonoforumSettings
        {
            AllowPublicTopics = monoforum.Settings.AllowPublicTopics,
        };
        
        var command = new CreateMonoforumTopicCommand(
            new ChannelId(request.ChannelId.ToString()),
            topicId,
            request.CreatorId,
            request.Title,
            request.Description,
            request.IsPublic,
            requiresApproval,
            "",
            false)
        {
            TopicId = topicId,
            ChannelId = request.ChannelId,
            CreatorId = request.CreatorId,
            Title = request.Title,
            Description = request.Description,
            IsPublic = true,
            IsAnonymous = request.IsAnonymous,
            Tags = string.IsNullOrEmpty(request.Tags) ? string.Empty : request.Tags,
            CreatedAt = DateTime.UtcNow,
            Status = requiresApproval ? MonoforumTopicStatus.Pending : MonoforumTopicStatus.Active,
            RequiresApproval = requiresApproval
        };

        await commandBus.PublishAsync<ChannelAggregate, ChannelId, IExecutionResult>(command, CancellationToken.None);

        var topic = new MonoforumTopic
        {
            Id = topicId,
            ChannelId = request.ChannelId,
            CreatorId = request.CreatorId,
            Title = request.Title,
            Description = request.Description,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            IsPublic = command.IsPublic,
            Status = command.Status,
            RequiresApproval = requiresApproval,
            Tags = new List<string>()
        };

        // Add creator as participant
        await JoinTopicAsync(topicId, request.CreatorId, request.IsAnonymous);

        // Add initial participants
        foreach (var participantId in request.InitialParticipants)
        {
            await JoinTopicAsync(topicId, participantId, false);
        }

        logger.LogInformation("Topic '{Title}' created successfully in monoforum channel {ChannelId}", 
            request.Title, request.ChannelId);

        return new MonoforumTopicResult
        {
            Success = true,
            TopicId = topicId,
            Topic = new MyTelegram.Domain.Shared.Forums.MonoforumTopic
            {
                Id = topic.Id,
                ChannelId = topic.ChannelId,
                CreatorId = topic.CreatorId,
                Title = topic.Title,
                Description = topic.Description,
                Tags = topic.Tags != null ? string.Join(",", topic.Tags) : string.Empty,
                CreatedAt = topic.CreatedAt,
                Status = (MyTelegram.Domain.Shared.Forums.MonoforumTopicStatus)(int)topic.Status,
                IsPublic = topic.IsPublic
            },
            RequiresApproval = requiresApproval,
            CreatedImmediately = !requiresApproval
        };
    }

    public async Task<object?> GetTopicAsync(string topicId)
    {
        var topic = await queryProcessor.ProcessAsync(new GetMonoforumTopicQuery(topicId));
        return new MyTelegram.Domain.Shared.Forums.MonoforumTopic
        {
            Id = topic.Id,
            ChannelId = topic.ChannelId,
            CreatorId = topic.CreatorId,
            Title = topic.Title,
            Description = topic.Description,
            Tags = topic.Tags != null ? string.Join(",", topic.Tags) : string.Empty,
            CreatedAt = topic.CreatedAt,
            Status = (MyTelegram.Domain.Shared.Forums.MonoforumTopicStatus)(int)topic.Status,
            IsPublic = topic.IsPublic
        };
    }

    public async Task<object> GetChannelTopicsAsync(long channelId, int offset = 0, int limit = 50)
    {
        var topics = await queryProcessor.ProcessAsync(
            new GetMonoforumTopicsQuery(channelId, offset, limit));

        return topics?.ToList() ?? new List<MonoforumTopic>();
    }

    public async Task<object> GetTopicsByUserAsync(long userId, int offset = 0, int limit = 50)
    {
        var topics = await queryProcessor.ProcessAsync(
            new GetMonoforumTopicsByUserQuery(userId, offset, limit));

        return topics?.ToList() ?? new List<MonoforumTopic>();
    }

    public async Task<bool> JoinTopicAsync(string topicId, long userId, bool isAnonymous = false)
    {
        logger.LogInformation("User {UserId} joining topic {TopicId}", userId, topicId);

        var topicObj = await GetTopicAsync(topicId);
        if (topicObj is not DomainMonoforumTopic topic)
        {
            return false;
        }

        // Check if user is already a participant
        var existingParticipant = await queryProcessor.ProcessAsync(
            new GetMonoforumParticipantQuery(topicId, userId));

        if (existingParticipant != null)
        {
            return false; // Already a participant
        }

        var role = userId == topic.CreatorId ? MonoforumParticipantRole.Creator : MonoforumParticipantRole.Member;
        var command = new JoinMonoforumTopicCommand(
            new ChannelId(topic.ChannelId.ToString()),
            topicId,
            userId,
            role,
            isAnonymous,
            true // canPost
        )
        {
            JoinedAt = DateTime.UtcNow
        };

        await commandBus.PublishAsync<ChannelAggregate, ChannelId, IExecutionResult>(command, CancellationToken.None);

        logger.LogInformation("User {UserId} joined topic {TopicId} successfully", userId, topicId);
        return true;
    }

    public async Task<bool> LeaveTopicAsync(string topicId, long userId)
    {
        logger.LogInformation("User {UserId} leaving topic {TopicId}", userId, topicId);

        var topicObj = await GetTopicAsync(topicId);
        if (topicObj is not DomainMonoforumTopic topic)
        {
            return false;
        }

        var command = new LeaveMonoforumTopicCommand(ChannelId.With(topic.ChannelId.ToString()))
        {
            TopicId = topicId,
            UserId = userId,
            LeftAt = DateTime.UtcNow
        };

        await commandBus.PublishAsync<ChannelAggregate, ChannelId, IExecutionResult>(command, CancellationToken.None);

        logger.LogInformation("User {UserId} left topic {TopicId} successfully", userId, topicId);
        return true;
    }

    public async Task<object?> SendMessageAsync(string topicId, long userId, string content, object typeObj, bool isAnonymous = false)
    {
        var type = (MonoforumMessageType)typeObj;
        logger.LogInformation("Sending message to topic {TopicId} by user {UserId}", topicId, userId);

        var topicObj = await GetTopicAsync(topicId);
        if (topicObj is not DomainMonoforumTopic topic || topic.Status != MyTelegram.Domain.Shared.Forums.MonoforumTopicStatus.Active)
        {
            return null;
        }

        var monoforum = await GetMonoforumAsync(topic.ChannelId);
        if (monoforum == null)
        {
            return null;
        }

        // Check if user is participant
        var participant = await queryProcessor.ProcessAsync(
            new GetMonoforumParticipantQuery(topicId, userId));

        if (participant == null)
        {
            return null; // User is not a participant
        }

        // Validate message content
        var validationResult = await ValidateMessageAsync(content, type, new MyTelegram.Domain.Shared.Forums.MonoforumSettings());
        if (!validationResult.IsValid)
        {
            throw new InvalidOperationException(validationResult.ErrorMessage);
        }

        var messageId = Guid.NewGuid().ToString();

        var command = new CreateMonoforumMessageCommand(ChannelId.With(topic.ChannelId.ToString()))
        {
            MessageId = messageId,
            TopicId = topicId,
            SenderId = userId,
            Content = content,
            Type = type,
            SentAt = DateTime.UtcNow,
            IsAnonymous = isAnonymous,
            Status = monoforum.Settings.AutoModerateMessages ? MonoforumMessageStatus.Approved : MonoforumMessageStatus.Pending
        };

        await commandBus.PublishAsync<ChannelAggregate, ChannelId, IExecutionResult>(command, CancellationToken.None);

        var message = new MonoforumMessage
        {
            Id = messageId,
            TopicId = topicId,
            SenderId = userId,
            Content = content,
            Type = type,
            SentAt = DateTime.UtcNow,
            IsAnonymous = isAnonymous,
            Status = command.Status
        };

        logger.LogInformation("Message sent successfully to topic {TopicId} by user {UserId}", topicId, userId);
        return message;
    }

    public async Task<object> GetTopicMessagesAsync(string topicId, int offset = 0, int limit = 50)
    {
        var messages = await queryProcessor.ProcessAsync(
            new GetMonoforumMessagesQuery(topicId, offset, limit));

        return messages?.ToList() ?? new List<MonoforumMessage>();
    }

    public async Task<bool> DeleteMessageAsync(string messageId, long deletedBy, string? reason = null)
    {
        logger.LogInformation("Deleting message {MessageId} by user {UserId}", messageId, deletedBy);

        var message = await queryProcessor.ProcessAsync(new GetMonoforumMessageQuery(messageId));
        if (message == null)
        {
            return false;
        }

        var topicObj = await GetTopicAsync(message.TopicId);
        if (topicObj is not DomainMonoforumTopic topic)
        {
            return false;
        }

        // Check if user has permission to delete
        if (!await CanDeleteMessageAsync(deletedBy, message, topic))
        {
            throw new UnauthorizedAccessException("User doesn't have permission to delete this message");
        }

        var command = new DeleteMonoforumMessageCommand(new ChannelId(topic.ChannelId.ToString()))
        {
            MessageId = messageId,
            DeletedBy = deletedBy,
            DeletedAt = DateTime.UtcNow,
            Reason = reason
        };

        await commandBus.PublishAsync<ChannelAggregate, ChannelId, IExecutionResult>(command, CancellationToken.None);

        logger.LogInformation("Message {MessageId} deleted successfully by user {UserId}", messageId, deletedBy);
        return true;
    }

    public async Task<object> GetMonoforumStatisticsAsync(long channelId, DateTime? from = null, DateTime? to = null)
    {
        logger.LogInformation("Getting monoforum statistics for channel {ChannelId}", channelId);

        var query = new GetMonoforumStatisticsQuery(channelId);

        var stats = await queryProcessor.ProcessAsync(query);
        
        return stats ?? new MonoforumStatistics
        {
            TotalTopics = 0,
            ActiveTopics = 0,
            TotalMessages = 0,
            From = from ?? DateTime.UtcNow.AddDays(-30),
            To = to ?? DateTime.UtcNow
        };
    }

    public async Task<object> GetTopicStatisticsAsync(string topicId)
    {
        var stats = await queryProcessor.ProcessAsync(new GetMonoforumTopicStatisticsQuery(topicId));
        
        return stats ?? new MonoforumTopicStatistics
        {
            TopicId = topicId
        };
    }

    public async Task<bool> PerformModerationActionAsync(long channelId, long moderatorId, object actionObj, long targetUserId, string? topicId = null, string? messageId = null, string? reason = null)
    {
        var action = (MonoforumModerationType)actionObj;
        logger.LogInformation("Performing moderation action {Action} in channel {ChannelId} by moderator {ModeratorId}", 
            action, channelId, moderatorId);

        if (!await IsChannelAdminAsync(moderatorId, channelId))
        {
            throw new UnauthorizedAccessException("User is not channel admin");
        }

        var command = new PerformMonoforumModerationCommand(new MyTelegram.Domain.Aggregates.Channel.ChannelId(channelId.ToString()))
        {
            ChannelId = channelId,
            ModeratorId = moderatorId,
            Action = action,
            TargetUserId = targetUserId,
            TopicId = topicId,
            MessageId = messageId,
            Reason = reason,
            PerformedAt = DateTime.UtcNow
        };

        await commandBus.PublishAsync<ChannelAggregate, ChannelId, IExecutionResult>(command, CancellationToken.None);

        logger.LogInformation("Moderation action {Action} performed successfully in channel {ChannelId}", action, channelId);
        return true;
    }

    public async Task<bool> ReactToMessageAsync(string messageId, long userId, string emoji)
    {
        logger.LogInformation("User {UserId} reacting to message {MessageId} with {Emoji}", userId, messageId, emoji);

        var message = await queryProcessor.ProcessAsync(new GetMonoforumMessageQuery(messageId));
        if (message == null)
        {
            return false;
        }

        // Check if user is topic participant
        var participant = await queryProcessor.ProcessAsync(
            new GetMonoforumParticipantQuery(message.TopicId, userId));

        if (participant == null)
        {
            return false; // User is not a participant
        }

        var topicObj = await GetTopicAsync(message.TopicId);
        if (topicObj is not DomainMonoforumTopic topic)
        {
            return false;
        }

        var command = new ReactToMonoforumMessageCommand(new ChannelId(topic.ChannelId.ToString()))
        {
            MessageId = messageId,
            UserId = userId,
            Emoji = emoji,
            ReactedAt = DateTime.UtcNow
        };

        await commandBus.PublishAsync<ChannelAggregate, ChannelId, IExecutionResult>(command, CancellationToken.None);

        logger.LogInformation("Reaction added successfully to message {MessageId} by user {UserId}", messageId, userId);
        return true;
    }

    public async Task<object> GetModerationHistoryAsync(long channelId, int offset = 0, int limit = 50)
    {
        var actions = await queryProcessor.ProcessAsync(
            new GetMonoforumModerationHistoryQuery(channelId, offset, limit));

        return actions?.ToList() ?? new List<MonoforumModerationAction>();
    }

    public async Task<bool> ApproveTopicAsync(string topicId, long approvedBy)
    {
        logger.LogInformation("Approving topic {TopicId} by user {UserId}", topicId, approvedBy);

        var topicObj = await GetTopicAsync(topicId);
        if (topicObj is not DomainMonoforumTopic topic)
        {
            return false;
        }

        if (!await IsChannelAdminAsync(approvedBy, topic.ChannelId))
        {
            throw new UnauthorizedAccessException("User is not channel admin");
        }

        var command = new ApproveMonoforumTopicCommand(ChannelId.With(topic.ChannelId.ToString()))
        {
            TopicId = topicId,
            ApprovedBy = approvedBy,
            ApprovedAt = DateTime.UtcNow
        };

        await commandBus.PublishAsync<ChannelAggregate, ChannelId, IExecutionResult>(command, CancellationToken.None);

        logger.LogInformation("Topic {TopicId} approved successfully by user {UserId}", topicId, approvedBy);
        return true;
    }

    public async Task<bool> RejectTopicAsync(string topicId, long rejectedBy, string? reason = null)
    {
        logger.LogInformation("Rejecting topic {TopicId} by user {UserId}", topicId, rejectedBy);

        var topicObj = await GetTopicAsync(topicId);
        if (topicObj is not DomainMonoforumTopic topic)
        {
            return false;
        }

        if (!await IsChannelAdminAsync(rejectedBy, topic.ChannelId))
        {
            throw new UnauthorizedAccessException("User is not channel admin");
        }

        var command = new RejectMonoforumTopicCommand(ChannelId.With(topic.ChannelId.ToString()))
        {
            TopicId = topicId,
            RejectedBy = rejectedBy,
            Reason = reason,
            RejectedAt = DateTime.UtcNow
        };

        await commandBus.PublishAsync<ChannelAggregate, ChannelId, IExecutionResult>(command, CancellationToken.None);

        logger.LogInformation("Topic {TopicId} rejected successfully by user {UserId}", topicId, rejectedBy);
        return true;
    }

    public async Task<bool> CloseTopicAsync(string topicId, long closedBy, string? reason = null)
    {
        logger.LogInformation("Closing topic {TopicId} by user {UserId}", topicId, closedBy);

        var topicObj = await GetTopicAsync(topicId);
        if (topicObj is not DomainMonoforumTopic topic)
        {
            return false;
        }

        if (!await CanCloseTopicAsync(closedBy, topic))
        {
            throw new UnauthorizedAccessException("User doesn't have permission to close this topic");
        }

        var command = new CloseMonoforumTopicCommand(
            MyTelegram.Domain.Aggregates.Channel.ChannelId.With(topic.ChannelId.ToString()))
        {
            TopicId = topicId,
            ClosedBy = closedBy,
            ClosedAt = DateTime.UtcNow
        };

        await commandBus.PublishAsync<ChannelAggregate, ChannelId, IExecutionResult>(command, CancellationToken.None);

        logger.LogInformation("Topic {TopicId} closed successfully by user {UserId}", topicId, closedBy);
        return true;
    }

    private async Task<bool> IsChannelAdminAsync(long userId, long channelId)
    {
        // In a real implementation, this would check channel participant permissions
        return await Task.FromResult(true); // Placeholder
    }

    private async Task<TopicValidationResult> ValidateTopicRequestAsync(MyTelegram.Domain.Shared.Forums.MonoforumTopicRequest request, DomainMonoforum monoforum)
    {
        // Check title length
        if (string.IsNullOrWhiteSpace(request.Title) || request.Title.Length > 100)
        {
            return new TopicValidationResult
            {
                IsValid = false,
                ErrorMessage = "Title must be between 1 and 100 characters"
            };
        }

        // Check public topics restriction
        if (request.IsPublic && !monoforum.Settings.AllowPublicTopics)
        {
            return new TopicValidationResult
            {
                IsValid = false,
                ErrorMessage = "Public topics are not allowed in this monoforum"
            };
        }

        return new TopicValidationResult { IsValid = true };
    }

    private async Task<MessageValidationResult> ValidateMessageAsync(string content, MonoforumMessageType type, MyTelegram.Domain.Shared.Forums.MonoforumSettings settings)
    {
        // Check message length
        if (content.Length > settings.MaxMessageLength)
        {
            return new MessageValidationResult
            {
                IsValid = false,
                ErrorMessage = $"Message length cannot exceed {settings.MaxMessageLength} characters"
            };
        }

        // Check links restriction
        if (!settings.AllowLinks && ContainsLinks(content))
        {
            return new MessageValidationResult
            {
                IsValid = false,
                ErrorMessage = "Links are not allowed in this monoforum"
            };
        }

        // Check media restrictions
        if (type != MonoforumMessageType.Text && !settings.AllowMedia)
        {
            return new MessageValidationResult
            {
                IsValid = false,
                ErrorMessage = "Media is not allowed in this monoforum"
            };
        }

        return new MessageValidationResult { IsValid = true };
    }

    private async Task<bool> CanDeleteMessageAsync(long userId, MonoforumMessage message, MyTelegram.Domain.Shared.Forums.MonoforumTopic topic)
    {
        // User can delete their own message
        if (message.SenderId == userId)
        {
            return true;
        }

        // Channel admin can delete any message
        if (await IsChannelAdminAsync(userId, topic.ChannelId))
        {
            return true;
        }

        // Topic creator can delete any message
        if (topic.CreatorId == userId)
        {
            return true;
        }

        return false;
    }

    private async Task<bool> CanCloseTopicAsync(long userId, MyTelegram.Domain.Shared.Forums.MonoforumTopic topic)
    {
        // Topic creator can close their own topic
        if (topic.CreatorId == userId)
        {
            return true;
        }

        // Channel admin can close any topic
        return await IsChannelAdminAsync(userId, topic.ChannelId);
    }

    private static bool ContainsLinks(string content)
    {
        // Simple regex to detect URLs
        var urlPattern = @"http[s]?://(?:[a-zA-Z]|[0-9]|[$-_@.&+]|[!*\\(\\),]|(?:%[0-9a-fA-F][0-9a-fA-F]))+";
        return System.Text.RegularExpressions.Regex.IsMatch(content, urlPattern);
    }
}

// Internal command classes remain here

public class CreateMonoforumCommand : Command<ChannelAggregate, ChannelId>
{
    public CreateMonoforumCommand(ChannelId channelId, long channelIdValue) : base(channelId) 
    {
        ChannelId = channelIdValue;
    }
    public long ChannelId { get; set; }
    public long CreatorId { get; set; }
    public DateTime CreatedAt { get; set; }
    public MonoforumSettings Settings { get; set; } = new();
}

public class UpdateMonoforumSettingsCommand : Command<ChannelAggregate, ChannelId>
{
    public UpdateMonoforumSettingsCommand(ChannelId channelId) : base(channelId) { }
    public long ChannelId { get; set; }
    public MonoforumSettings Settings { get; set; } = new();
    public long UpdatedBy { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class CreateMonoforumTopicCommand : Command<ChannelAggregate, ChannelId>
{
    public CreateMonoforumTopicCommand(ChannelId channelId, string topicId, long creatorId, string title, string? description, bool isPublic, bool isAnonymous, string? tags, bool requiresApproval) 
        : base(channelId) 
    {
        TopicId = topicId;
        ChannelId = long.Parse(channelId.Value);
        CreatorId = creatorId;
        Title = title;
        Description = description;
        IsPublic = isPublic;
        IsAnonymous = isAnonymous;
        Tags = string.Join(",", tags);
        CreatedAt = DateTime.UtcNow;
        Status = MonoforumTopicStatus.Active;
        RequiresApproval = requiresApproval;
    }
    public string TopicId { get; set; } = string.Empty;
    public long ChannelId { get; set; }
    public long CreatorId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsPublic { get; set; }
    public bool IsAnonymous { get; set; }
    public string? Tags { get; set; }
    public DateTime CreatedAt { get; set; }
    public MonoforumTopicStatus Status { get; set; }
    public bool RequiresApproval { get; set; }
}

public class JoinMonoforumTopicCommand : Command<ChannelAggregate, ChannelId>
{
    public JoinMonoforumTopicCommand(ChannelId channelId, string topicId, long userId, MonoforumParticipantRole role, bool isAnonymous, bool canPost) 
        : base(channelId) 
    {
        TopicId = topicId;
        UserId = userId;
        JoinedAt = DateTime.UtcNow;
        Role = role;
        IsAnonymous = isAnonymous;
        CanPost = canPost;
    }
    public string TopicId { get; set; } = string.Empty;
    public long UserId { get; set; }
    public DateTime JoinedAt { get; set; }
    public MonoforumParticipantRole Role { get; set; }
    public bool IsAnonymous { get; set; }
    public bool CanPost { get; set; }
}

public class LeaveMonoforumTopicCommand : Command<ChannelAggregate, ChannelId>
{
    public LeaveMonoforumTopicCommand(ChannelId channelId) : base(channelId) { }
    public string TopicId { get; set; } = string.Empty;
    public long UserId { get; set; }
    public DateTime LeftAt { get; set; }
}

public class CreateMonoforumMessageCommand : Command<ChannelAggregate, ChannelId>
{
    public CreateMonoforumMessageCommand(ChannelId channelId) : base(channelId) { }
    public string MessageId { get; set; } = string.Empty;
    public string TopicId { get; set; } = string.Empty;
    public long SenderId { get; set; }
    public string Content { get; set; } = string.Empty;
    public MonoforumMessageType Type { get; set; }
    public DateTime SentAt { get; set; }
    public bool IsAnonymous { get; set; }
    public MonoforumMessageStatus Status { get; set; }
}

public class DeleteMonoforumMessageCommand : Command<ChannelAggregate, ChannelId>
{
    public DeleteMonoforumMessageCommand(ChannelId channelId) : base(channelId) { }
    public string MessageId { get; set; } = string.Empty;
    public long DeletedBy { get; set; }
    public DateTime DeletedAt { get; set; }
    public string? Reason { get; set; }
}

public class PerformMonoforumModerationCommand : Command<ChannelAggregate, ChannelId>
{
    public PerformMonoforumModerationCommand(ChannelId channelId) : base(channelId) { }
    public long ChannelId { get; set; }
    public long ModeratorId { get; set; }
    public MonoforumModerationType Action { get; set; }
    public long TargetUserId { get; set; }
    public string? TopicId { get; set; }
    public string? MessageId { get; set; }
    public string? Reason { get; set; }
    public DateTime PerformedAt { get; set; }
}

public class ReactToMonoforumMessageCommand : Command<ChannelAggregate, ChannelId>
{
    public ReactToMonoforumMessageCommand(ChannelId channelId) : base(channelId) { }
    public string MessageId { get; set; } = string.Empty;
    public long UserId { get; set; }
    public string Emoji { get; set; } = string.Empty;
    public DateTime ReactedAt { get; set; }
}

public class ApproveMonoforumTopicCommand : Command<ChannelAggregate, ChannelId>
{
    public ApproveMonoforumTopicCommand(ChannelId channelId) : base(channelId) { }
    public string TopicId { get; set; } = string.Empty;
    public long ApprovedBy { get; set; }
    public DateTime ApprovedAt { get; set; }
}

public class RejectMonoforumTopicCommand : Command<ChannelAggregate, ChannelId>
{
    public RejectMonoforumTopicCommand(ChannelId channelId) : base(channelId) { }
    public string TopicId { get; set; } = string.Empty;
    public long RejectedBy { get; set; }
    public string? Reason { get; set; }
    public DateTime RejectedAt { get; set; }
}

public class CloseMonoforumTopicCommand : Command<ChannelAggregate, ChannelId>
{
public CloseMonoforumTopicCommand(ChannelId channelId) : base(channelId) { }
public string TopicId { get; set; } = string.Empty;
public long ClosedBy { get; set; }
public string? Reason { get; set; }
public DateTime ClosedAt { get; set; }
}
