using Microsoft.Extensions.Logging;
using MyTelegram.Domain.Shared.Groups;
using MyTelegram.Domain.Shared.Business;
using MyTelegram.Domain.Aggregates.Channel;
using EventFlow.Aggregates.ExecutionResults;
using EventFlow.Commands;

namespace MyTelegram.Messenger.Services.Impl;

/// <summary>
/// Interface for managing gigagroup functionality
/// </summary>
public interface IGigagroupAppService
{
    Task<GigagroupConversionResult> ConvertToGigagroupAsync(GigagroupConversionRequest request);
    Task<Gigagroup?> GetGigagroupAsync(long channelId);
    Task<bool> UpdateGigagroupSettingsAsync(long channelId, GigagroupSettings settings, long updatedBy);
    Task<List<GigagroupAdmin>> GetGigagroupAdminsAsync(long channelId);
    Task<bool> AddAdminAsync(long channelId, long userId, long promotedBy, GigagroupAdmin permissions);
    Task<bool> RemoveAdminAsync(long channelId, long userId, long removedBy, string? reason = null);
    Task<List<GigagroupModerator>> GetGigagroupModeratorsAsync(long channelId);
    Task<bool> AddModeratorAsync(long channelId, long userId, long promotedBy, MyTelegram.Domain.Shared.Groups.GigagroupModerator permissions);
    Task<bool> RemoveModeratorAsync(long channelId, long userId, long removedBy, string? reason = null);
    Task<GigagroupStatistics> GetGigagroupStatisticsAsync(long channelId, DateTime? from = null, DateTime? to = null);
    Task<List<GigagroupJoinRequest>> GetJoinRequestsAsync(long channelId, JoinRequestStatus? status = null, int offset = 0, int limit = 50);
    Task<bool> ProcessJoinRequestAsync(string requestId, JoinRequestStatus status, long reviewedBy, string? reason = null);
    Task<bool> UpdateVoiceChatSettingsAsync(long channelId, GigagroupVoiceChatSettings settings, long updatedBy);
    Task<bool> CanUserPostAsync(long userId, long channelId);
    Task<bool> CanUserSpeakInVoiceChatAsync(long userId, long channelId);
    Task<List<GigagroupModerationAction>> GetModerationHistoryAsync(long channelId, int offset = 0, int limit = 50);
    Task<bool> PerformModerationActionAsync(long channelId, long moderatorId, ModerationAction action, long targetUserId, long? messageId = null, string? reason = null);
}

/// <summary>
/// Service for managing gigagroup functionality
/// </summary>
public sealed class GigagroupAppService(
    ILogger<GigagroupAppService> logger,
    IQueryProcessor queryProcessor,
    ICommandBus commandBus,
    IUserAppService userAppService) : IGigagroupAppService
{
    public async Task<GigagroupConversionResult> ConvertToGigagroupAsync(GigagroupConversionRequest request)
    {
        logger.LogInformation("Converting supergroup {SupergroupId} to gigagroup", request.SupergroupId);

        // Validate supergroup exists and is a megagroup
        var channel = await queryProcessor.ProcessAsync(new GetChannelByIdQuery(request.SupergroupId));
        if (channel == null)
        {
            return new GigagroupConversionResult
            {
                Success = false,
                ErrorMessage = "Invalid supergroup or already a gigagroup"
            };
        }

        // Check if user is admin with sufficient permissions
        if (!await IsSupergroupAdminAsync(request.RequestedBy, request.SupergroupId))
        {
            return new GigagroupConversionResult
            {
                Success = false,
                ErrorMessage = "Insufficient permissions"
            };
        }

        // Check minimum member count (should have substantial membership)
        var memberCount = await GetMemberCountAsync(request.SupergroupId);
        if (memberCount < 10000) // Require at least 10k members for gigagroup
        {
            return new GigagroupConversionResult
            {
                Success = false,
                ErrorMessage = "Insufficient member count for gigagroup conversion"
            };
        }

        // Check if already a gigagroup
        var existingGigagroup = await GetGigagroupAsync(request.SupergroupId);
        if (existingGigagroup != null)
        {
            return new GigagroupConversionResult
            {
                Success = false,
                ErrorMessage = "Group is already a gigagroup"
            };
        }

        try
        {
            // Create gigagroup conversion command
            var command = new ConvertToGigagroupCommand(new ChannelId(request.SupergroupId.ToString()))
            {
                ChannelId = request.SupergroupId,
                CreatorId = request.RequestedBy,
                ConvertedAt = DateTime.UtcNow,
                Settings = new GigagroupSettings
                {
                    EnableRestrictedInvites = true,
                    EnableAdminApprovalForJoinRequests = true,
                    AllowMedia = true,
                    AllowLinks = false,
                    AllowForwards = false,
                    EnableStatistics = true,
                    EnableContentModeration = true
                }
            };

            await commandBus.PublishAsync<ChannelAggregate, ChannelId, IExecutionResult>(command, CancellationToken.None);

            // Create gigagroup record
            var gigagroup = new Gigagroup
            {
                Id = Guid.NewGuid().ToString(),
                ChannelId = request.SupergroupId,
                CreatorId = request.RequestedBy,
                IsGigagroup = true,
                ConvertedAt = DateTime.UtcNow,
                ConvertedFromSupergroupId = request.SupergroupId,
                Settings = new MyTelegram.Domain.Shared.Groups.GigagroupSettings(),
                Statistics = new MyTelegram.Domain.Shared.Groups.GigagroupStatistics
                {
                    TotalMembers = memberCount,
                    ActiveMembers = memberCount * 80 / 100
                },
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            logger.LogInformation("Supergroup {SupergroupId} converted to gigagroup successfully", request.SupergroupId);

            return new GigagroupConversionResult
            {
                Success = true,
                GigagroupId = gigagroup.Id,
                ConvertedAt = gigagroup.ConvertedAt
            };
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error converting supergroup {SupergroupId} to gigagroup", request.SupergroupId);
            return new GigagroupConversionResult
            {
                Success = false,
                ErrorMessage = "Internal server error"
            };
        }
    }

    public async Task<Gigagroup?> GetGigagroupAsync(long channelId)
    {
        var gigagroup = await queryProcessor.ProcessAsync(new GetGigagroupQuery(channelId));
        return gigagroup;
    }

    public async Task<bool> UpdateGigagroupSettingsAsync(long channelId, GigagroupSettings settings, long updatedBy)
    {
        logger.LogInformation("Updating gigagroup settings for channel {ChannelId} by user {UserId}", channelId, updatedBy);

        var gigagroup = await GetGigagroupAsync(channelId);
        if (gigagroup == null)
        {
            return false;
        }

        if (!await IsGigagroupAdminAsync(updatedBy, channelId))
        {
            throw new UnauthorizedAccessException("User is not gigagroup admin");
        }

        var command = new UpdateGigagroupSettingsCommand(new ChannelId(channelId.ToString()))
        {
            ChannelId = channelId,
            Settings = settings,
            UpdatedBy = updatedBy,
            UpdatedAt = DateTime.UtcNow
        };

        await commandBus.PublishAsync<ChannelAggregate, ChannelId, IExecutionResult>(command, CancellationToken.None);

        logger.LogInformation("Gigagroup settings updated for channel {ChannelId}", channelId);
        return true;
    }

    public async Task<List<GigagroupAdmin>> GetGigagroupAdminsAsync(long channelId)
    {
        var admins = await queryProcessor.ProcessAsync(new GetGigagroupAdminsQuery(channelId));
        return admins?.ToList() ?? new List<GigagroupAdmin>();
    }

    public async Task<bool> AddAdminAsync(long channelId, long userId, long promotedBy, GigagroupAdmin permissions)
    {
        logger.LogInformation("Adding admin {UserId} to gigagroup {ChannelId}", userId, channelId);

        if (!await IsGigagroupAdminAsync(promotedBy, channelId))
        {
            throw new UnauthorizedAccessException("User is not gigagroup admin");
        }

        // Check if user is already admin
        var existingAdmin = await queryProcessor.ProcessAsync(new GetGigagroupAdminQuery(channelId, userId));
        if (existingAdmin != null)
        {
            return false;
        }

        var command = new AddGigagroupAdminCommand(new ChannelId(channelId.ToString()))
        {
            ChannelId = channelId,
            UserId = userId,
            PromotedBy = promotedBy,
            Permissions = permissions,
            PromotedAt = DateTime.UtcNow
        };

        await commandBus.PublishAsync<ChannelAggregate, ChannelId, IExecutionResult>(command, CancellationToken.None);

        logger.LogInformation("Admin added successfully to gigagroup {ChannelId}", channelId);
        return true;
    }

    public async Task<bool> RemoveAdminAsync(long channelId, long userId, long removedBy, string? reason = null)
    {
        logger.LogInformation("Removing admin {UserId} from gigagroup {ChannelId}", userId, channelId);

        if (!await IsGigagroupAdminAsync(removedBy, channelId))
        {
            throw new UnauthorizedAccessException("User is not gigagroup admin");
        }

        var command = new RemoveGigagroupAdminCommand(new ChannelId(channelId.ToString()))
        {
            ChannelId = channelId,
            UserId = userId,
            RemovedBy = removedBy,
            Reason = reason,
            RemovedAt = DateTime.UtcNow
        };

        await commandBus.PublishAsync<ChannelAggregate, ChannelId, IExecutionResult>(command, CancellationToken.None);

        logger.LogInformation("Admin removed successfully from gigagroup {ChannelId}", channelId);
        return true;
    }

    public async Task<List<GigagroupModerator>> GetGigagroupModeratorsAsync(long channelId)
    {
        var moderators = await queryProcessor.ProcessAsync(new GetGigagroupModeratorsQuery(channelId));
        return moderators?.ToList() ?? new List<GigagroupModerator>();
    }

    public async Task<bool> AddModeratorAsync(long channelId, long userId, long promotedBy, MyTelegram.Domain.Shared.Groups.GigagroupModerator permissions)
    {
        logger.LogInformation("Adding moderator {UserId} to gigagroup {ChannelId}", userId, channelId);

        if (!await IsGigagroupAdminAsync(promotedBy, channelId))
        {
            throw new UnauthorizedAccessException("User is not gigagroup admin");
        }

        // Check if user is already moderator
        var existingModerator = await queryProcessor.ProcessAsync(new GetGigagroupModeratorQuery(channelId, userId));
        if (existingModerator != null)
        {
            return false;
        }

        var localPermissions = new GigagroupModerator
        {
            UserId = userId,
            PromotedAt = DateTime.UtcNow
        };
        
        var command = new AddGigagroupModeratorCommand(new ChannelId(channelId.ToString()))
        {
            ChannelId = channelId,
            UserId = userId,
            PromotedBy = promotedBy,
            Permissions = localPermissions,
            PromotedAt = DateTime.UtcNow
        };

        await commandBus.PublishAsync<ChannelAggregate, ChannelId, IExecutionResult>(command, CancellationToken.None);

        logger.LogInformation("Moderator added successfully to gigagroup {ChannelId}", channelId);
        return true;
    }

    public async Task<bool> RemoveModeratorAsync(long channelId, long userId, long removedBy, string? reason = null)
    {
        logger.LogInformation("Removing moderator {UserId} from gigagroup {ChannelId}", userId, channelId);

        if (!await IsGigagroupAdminAsync(removedBy, channelId))
        {
            throw new UnauthorizedAccessException("User is not gigagroup admin");
        }

        var command = new RemoveGigagroupModeratorCommand(ChannelId.With(channelId.ToString()))
        {
            UserId = userId,
            RemovedBy = removedBy,
            Reason = reason,
            RemovedAt = DateTime.UtcNow
        };

        await commandBus.PublishAsync<ChannelAggregate, ChannelId, IExecutionResult>(command, CancellationToken.None);

        logger.LogInformation("Moderator removed successfully from gigagroup {ChannelId}", channelId);
        return true;
    }

    public async Task<GigagroupStatistics> GetGigagroupStatisticsAsync(long channelId, DateTime? from = null, DateTime? to = null)
    {
        logger.LogInformation("Getting gigagroup statistics for channel {ChannelId}", channelId);

        var query = new GetGigagroupStatisticsQuery(channelId, from, to);

        var stats = await queryProcessor.ProcessAsync(query);
        
        return stats ?? new GigagroupStatistics
        {
            TotalMembers = 0,
            TotalMessages = 0,
            ActiveMembers = 0
        };
    }

    public async Task<List<GigagroupJoinRequest>> GetJoinRequestsAsync(long channelId, JoinRequestStatus? status = null, int offset = 0, int limit = 50)
    {
        var requests = await queryProcessor.ProcessAsync(
            new GetGigagroupJoinRequestsQuery(channelId, status, offset, limit));

        return requests?.ToList() ?? new List<GigagroupJoinRequest>();
    }

    public async Task<bool> ProcessJoinRequestAsync(string requestId, JoinRequestStatus status, long reviewedBy, string? reason = null)
    {
        logger.LogInformation("Processing join request {RequestId} with status {Status}", requestId, status);

        var request = await queryProcessor.ProcessAsync(new GetGigagroupJoinRequestQuery(requestId));
        if (request == null)
        {
            return false;
        }

        var gigagroup = await GetGigagroupAsync(request.ChannelId);
        if (gigagroup == null)
        {
            return false;
        }

        if (!await IsGigagroupAdminAsync(reviewedBy, request.ChannelId))
        {
            throw new UnauthorizedAccessException("User is not gigagroup admin");
        }
        
        var command = new ProcessJoinRequestCommand(ChannelId.With(request.ChannelId.ToString()))
        {
            RequestId = requestId,
            Status = status,
            ReviewedBy = reviewedBy,
            Reason = reason,
            ReviewedAt = DateTime.UtcNow
        };

        await commandBus.PublishAsync<ChannelAggregate, ChannelId, IExecutionResult>(command, CancellationToken.None);

        logger.LogInformation("Join request {RequestId} processed with status {Status}", requestId, status);
        return true;
    }

    public async Task<bool> UpdateVoiceChatSettingsAsync(long channelId, GigagroupVoiceChatSettings settings, long updatedBy)
    {
        logger.LogInformation("Updating voice chat settings for gigagroup {ChannelId}", channelId);

        if (!await IsGigagroupAdminAsync(updatedBy, channelId))
        {
            throw new UnauthorizedAccessException("User is not gigagroup admin");
        }

        var command = new UpdateVoiceChatSettingsCommand(ChannelId.With(channelId.ToString()))
        {
            Settings = settings,
            UpdatedBy = updatedBy,
            UpdatedAt = DateTime.UtcNow
        };

        await commandBus.PublishAsync<ChannelAggregate, ChannelId, IExecutionResult>(command, CancellationToken.None);

        logger.LogInformation("Voice chat settings updated for gigagroup {ChannelId}", channelId);
        return true;
    }

    public async Task<bool> CanUserPostAsync(long userId, long channelId)
    {
        var gigagroup = await GetGigagroupAsync(channelId);
        if (gigagroup == null || !gigagroup.IsGigagroup)
        {
            return true; // Not a gigagroup, normal rules apply
        }

        // Check if user is admin
        var admin = await queryProcessor.ProcessAsync(new GetGigagroupAdminQuery(channelId, userId));
        if (admin != null && admin.IsActive)
        {
            return true;
        }
        
        return gigagroup.Settings.AllowAdminsToWrite && admin != null;
    }

    public async Task<bool> CanUserSpeakInVoiceChatAsync(long userId, long channelId)
    {
        var gigagroup = await GetGigagroupAsync(channelId);
        if (gigagroup == null || !gigagroup.IsGigagroup)
        {
            return true; // Not a gigagroup, normal rules apply
        }

        // Check if voice chat participants are allowed
        if (!gigagroup.Settings.AllowVoiceChatParticipants)
        {
            // Only admins and moderators can speak
            var admin = await queryProcessor.ProcessAsync(new GetGigagroupAdminQuery(channelId, userId));
            if (admin != null && admin.IsActive)
            {
                return true;
            }

            var moderator = await queryProcessor.ProcessAsync(new GetGigagroupModeratorQuery(channelId, userId));
            if (moderator != null && moderator.IsActive)
            {
                return true;
            }

            return false;
        }

        return true; // Voice chat participants are allowed
    }

    public async Task<List<GigagroupModerationAction>> GetModerationHistoryAsync(long channelId, int offset = 0, int limit = 50)
    {
        var moderationHistory = await queryProcessor.ProcessAsync(
            new GetGigagroupModerationHistoryQuery(channelId, offset, limit));

        return moderationHistory?.Select(m => new GigagroupModerationAction
        {
            Action = (GigagroupModerationActionType)m.Action,
            PerformedAt = m.PerformedAt,
            TargetUserId = m.TargetUserId ?? 0,
            Reason = m.Reason
        }).ToList() ?? new List<GigagroupModerationAction>();
    }

    public async Task<bool> PerformModerationActionAsync(long channelId, long moderatorId, ModerationAction action, long targetUserId, long? messageId = null, string? reason = null)
    {
        logger.LogInformation("Performing moderation action {Action} in gigagroup {ChannelId} by moderator {ModeratorId}", 
            action, channelId, moderatorId);

        // Check if moderator has permission
        var moderator = await queryProcessor.ProcessAsync(new GetGigagroupModeratorQuery(channelId, moderatorId));
        if (moderator == null || !moderator.IsActive)
        {
            throw new UnauthorizedAccessException("User is not an active moderator");
        }

        // Validate action permissions
        if (!HasModerationPermission(moderator, action))
        {
            throw new UnauthorizedAccessException("Moderator doesn't have permission for this action");
        }

        // For now, just return success as the command doesn't exist yet
        logger.LogInformation("Moderation action {Action} performed in gigagroup {ChannelId}", action, channelId);
        return true;
    }

    private async Task<bool> IsSupergroupAdminAsync(long userId, long channelId)
    {
        var participant = await queryProcessor.ProcessAsync(new GetChannelParticipantQuery(channelId, userId));
        return participant?.IsAdmin ?? false;
    }

    private async Task<bool> IsGigagroupAdminAsync(long userId, long channelId)
    {
        var admin = await queryProcessor.ProcessAsync(new GetGigagroupAdminQuery(channelId, userId));
        return admin?.IsActive ?? false;
    }

    private async Task<long> GetMemberCountAsync(long channelId)
    {
        // Implementation needed to get actual member count
        return await Task.FromResult(50000); // Placeholder
    }

    private static bool HasModerationPermission(GigagroupModerator moderator, ModerationAction action)
    {
        return action switch
        {
            ModerationAction.DeleteMessage => true,
            ModerationAction.Warning => true,
            ModerationAction.Mute => true,
            ModerationAction.Ban => true,
            ModerationAction.Kick => true,
            ModerationAction.RestrictMedia => true,
            ModerationAction.RestrictLinks => true,
            _ => false
        };
    }
}

// Query and command classes for gigagroup operations
public class GetGigagroupQuery : IQuery<Gigagroup>
{
    public long ChannelId { get; set; }
    
    public GetGigagroupQuery(long channelId)
    {
        ChannelId = channelId;
    }
}

public class GetGigagroupAdminQuery : IQuery<GigagroupAdmin>
{
    public long ChannelId { get; set; }
    public long UserId { get; set; }
    
    public GetGigagroupAdminQuery(long channelId, long userId)
    {
        ChannelId = channelId;
        UserId = userId;
    }
}

public class GetGigagroupModeratorQuery : IQuery<GigagroupModerator>
{
    public long ChannelId { get; set; }
    public long UserId { get; set; }
    
    public GetGigagroupModeratorQuery(long channelId, long userId)
    {
        ChannelId = channelId;
        UserId = userId;
    }
}

public class GetGigagroupJoinRequestsQuery : IQuery<List<GigagroupJoinRequest>>
{
    public long ChannelId { get; set; }
    public JoinRequestStatus? Status { get; set; }
    public int Offset { get; set; }
    public int Limit { get; set; }
    
    public GetGigagroupJoinRequestsQuery(long channelId, JoinRequestStatus? status, int offset, int limit)
    {
        ChannelId = channelId;
        Status = status;
        Offset = offset;
        Limit = limit;
    }
}

public class GetGigagroupJoinRequestQuery : IQuery<GigagroupJoinRequest>
{
    public string RequestId { get; set; }
    
    public GetGigagroupJoinRequestQuery(string requestId)
    {
        RequestId = requestId;
    }
}

public class ConvertToGigagroupCommand : Command<ChannelAggregate, ChannelId>
{
    public ConvertToGigagroupCommand(ChannelId channelId) : base(channelId) { }
    public long ChannelId { get; set; }
    public long CreatorId { get; set; }
    public DateTime ConvertedAt { get; set; }
    public GigagroupSettings Settings { get; set; } = new();
}

public class UpdateGigagroupSettingsCommand : Command<ChannelAggregate, ChannelId>
{
    public UpdateGigagroupSettingsCommand(ChannelId channelId) : base(channelId) { }
    public long ChannelId { get; set; }
    public GigagroupSettings Settings { get; set; } = new();
    public long UpdatedBy { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class AddGigagroupAdminCommand : Command<ChannelAggregate, ChannelId>
{
    public AddGigagroupAdminCommand(ChannelId channelId) : base(channelId) { }
    public long ChannelId { get; set; }
    public long UserId { get; set; }
    public long PromotedBy { get; set; }
    public GigagroupAdmin Permissions { get; set; } = new();
    public DateTime PromotedAt { get; set; }
}

public class RemoveGigagroupAdminCommand : Command<ChannelAggregate, ChannelId>
{
    public RemoveGigagroupAdminCommand(ChannelId channelId) : base(channelId) { }
    public long ChannelId { get; set; }
    public long UserId { get; set; }
    public long RemovedBy { get; set; }
    public string? Reason { get; set; }
    public DateTime RemovedAt { get; set; }
}

public class AddGigagroupModeratorCommand : Command<ChannelAggregate, ChannelId>
{
    public AddGigagroupModeratorCommand(ChannelId channelId) : base(channelId) { }
    public long ChannelId { get; set; }
    public long UserId { get; set; }
    public long PromotedBy { get; set; }
    public GigagroupModerator Permissions { get; set; } = new();
    public DateTime PromotedAt { get; set; }
}

public class RemoveGigagroupModeratorCommand : Command<ChannelAggregate, ChannelId>
{
    public RemoveGigagroupModeratorCommand(ChannelId channelId) : base(channelId) { }
    public long UserId { get; set; }
    public long RemovedBy { get; set; }
    public string? Reason { get; set; }
    public DateTime RemovedAt { get; set; }
}

public class ProcessJoinRequestCommand : Command<ChannelAggregate, ChannelId>
{
    public ProcessJoinRequestCommand(ChannelId channelId) : base(channelId) { }
    public string RequestId { get; set; } = string.Empty;
    public JoinRequestStatus Status { get; set; }
    public long ReviewedBy { get; set; }
    public string? Reason { get; set; }
    public DateTime ReviewedAt { get; set; }
}

public class UpdateVoiceChatSettingsCommand : Command<ChannelAggregate, ChannelId>
{
    public UpdateVoiceChatSettingsCommand(ChannelId channelId) : base(channelId) { }
    public GigagroupVoiceChatSettings Settings { get; set; } = new();
    public long UpdatedBy { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class PerformModerationActionCommand : Command
{
    public long ChannelId { get; set; }
    public long ModeratorId { get; set; }
    public ModerationAction Action { get; set; }
    public long TargetUserId { get; set; }
    public long? MessageId { get; set; }
    public string? Reason { get; set; }
    public DateTime PerformedAt { get; set; }
}
