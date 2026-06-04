using MyTelegram.Domain.Shared.Forums;
using MyTelegram.Domain.Shared.Business;

namespace MyTelegram.Messenger.Services;

// Alias to avoid conflicts between CommandQueryBase types and Domain.Shared types
using DomainMonoforum = MyTelegram.Domain.Shared.Forums.Monoforum;
using DomainMonoforumSettings = MyTelegram.Domain.Shared.Forums.MonoforumSettings;
using DomainMonoforumTopic = MyTelegram.Domain.Shared.Forums.MonoforumTopic;
using ServiceMonoforumTopic = MyTelegram.Messenger.Services.Impl.MonoforumTopic;

/// <summary>
/// Interface for managing monoforum functionality (simplified for Paid Messages feature)
/// </summary>
public interface IMonoforumAppService
{
    // Core methods needed for Paid Messages feature
    Task<DomainMonoforum> CreateMonoforumAsync(CreateMonoforumRequest request);
    Task<DomainMonoforum?> GetMonoforumAsync(long channelId);
    
    // Full implementation - using object to avoid type conflicts for now
    Task<bool> UpdateMonoforumSettingsAsync(long channelId, DomainMonoforumSettings settings, long updatedBy);
    Task<object> CreateTopicAsync(object request);
    Task<object?> GetTopicAsync(string topicId);
    Task<object> GetChannelTopicsAsync(long channelId, int offset = 0, int limit = 50);
    Task<object> GetTopicsByUserAsync(long userId, int offset = 0, int limit = 50);
    Task<bool> JoinTopicAsync(string topicId, long userId, bool isAnonymous = false);
    Task<bool> LeaveTopicAsync(string topicId, long userId);
    Task<object?> SendMessageAsync(string topicId, long userId, string content, object type, bool isAnonymous = false);
    Task<object> GetTopicMessagesAsync(string topicId, int offset = 0, int limit = 50);
    Task<bool> DeleteMessageAsync(string messageId, long deletedBy, string? reason = null);
    Task<object> GetMonoforumStatisticsAsync(long channelId, DateTime? from = null, DateTime? to = null);
    Task<object> GetTopicStatisticsAsync(string topicId);
    Task<bool> PerformModerationActionAsync(long channelId, long moderatorId, object action, long targetUserId, string? topicId = null, string? messageId = null, string? reason = null);
    Task<bool> ReactToMessageAsync(string messageId, long userId, string emoji);
    Task<object> GetModerationHistoryAsync(long channelId, int offset = 0, int limit = 50);
    Task<bool> ApproveTopicAsync(string topicId, long approvedBy);
    Task<bool> RejectTopicAsync(string topicId, long rejectedBy, string? reason = null);
    Task<bool> CloseTopicAsync(string topicId, long closedBy, string? reason = null);
}
