using MyTelegram.Domain.Shared.Business;
using MyTelegram.Schema;
using MyTelegram.Domain.Aggregates.PeerSetting;
using MyTelegram.Domain.Aggregates.Temp;
using EventFlow.Commands;
using MyTelegram.Domain.Shared;
using MyTelegram.Domain.Shared.Affiliate;
using MyTelegram.Domain.Shared.Messages;
using EventFlow.Aggregates.ExecutionResults;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Buffers;

namespace MyTelegram.Messenger.Services.Impl;

// Missing interfaces
public interface ICommand<TResult>
{
}

// Base Query Classes
public class GetUserFullQuery : IQuery<IUserFullReadModel>
{
    public long UserId { get; set; }

    public GetUserFullQuery(long userId)
    {
        UserId = userId;
    }
}

public class GetContactTypeQuery
{
    public long UserId { get; set; }
    public long PeerId { get; set; }

    public GetContactTypeQuery(long userId, long peerId)
    {
        UserId = userId;
        PeerId = peerId;
    }
}

// Enums
public enum ContactType
{
    None,
    Mutual,
    TargetUserIsMyContact,
    Self,
    TargetUserIsNotMyContact
}

// Missing query classes
public class GetPaidMediaPurchasesByMediaIdQuery : IQuery<List<PaidMediaPurchase>>
{
    public string MediaId { get; set; }
    
    public GetPaidMediaPurchasesByMediaIdQuery(string mediaId)
    {
        MediaId = mediaId;
    }
}

public class GetChannelParticipantQuery : IQuery<ChannelParticipant>
{
    public long ChannelId { get; set; }
    public long UserId { get; set; }
    
    public GetChannelParticipantQuery(long channelId, long userId)
    {
        ChannelId = channelId;
        UserId = userId;
    }
}

public class GetReferralByUserQuery : IQuery<Referral>
{
    public long UserId { get; set; }
    
    public GetReferralByUserQuery(long userId)
    {
        UserId = userId;
    }
}

public class GetAffiliateStatsQuery : IQuery<AffiliateStats>
{
    public long AffiliateId { get; set; }
    public long BotId { get; set; }
    public DateTime PeriodStart { get; set; }
    public DateTime PeriodEnd { get; set; }
    
    public GetAffiliateStatsQuery(long affiliateId, long botId)
    {
        AffiliateId = affiliateId;
        BotId = botId;
    }
}

public class GetAffiliateReferralsQuery : IQuery<List<Referral>>
{
    public long AffiliateId { get; set; }
    public long BotId { get; set; }
    public int Offset { get; set; }
    public int Limit { get; set; }
    
    public GetAffiliateReferralsQuery(long affiliateId, long botId, int offset = 0, int limit = 50)
    {
        AffiliateId = affiliateId;
        BotId = botId;
        Offset = offset;
        Limit = limit;
    }
}

public class GetAffiliateCommissionsQuery : IQuery<List<AffiliateCommission>>
{
    public long AffiliateId { get; set; }
    public long BotId { get; set; }
    public int Offset { get; set; }
    public int Limit { get; set; }
    
    public GetAffiliateCommissionsQuery(long affiliateId, long botId, int offset = 0, int limit = 50)
    {
        AffiliateId = affiliateId;
        BotId = botId;
        Offset = offset;
        Limit = limit;
    }
}

public class GetAffiliateApprovedCommissionsQuery : IQuery<List<AffiliateCommission>>
{
    public long AffiliateId { get; set; }
    public long BotId { get; set; }
    
    public GetAffiliateApprovedCommissionsQuery(long affiliateId, long botId)
    {
        AffiliateId = affiliateId;
        BotId = botId;
    }
}

public class GetAffiliatePayoutsQuery : IQuery<List<AffiliatePayout>>
{
    public long AffiliateId { get; set; }
    public long BotId { get; set; }
    public int Offset { get; set; }
    public int Limit { get; set; }
    
    public GetAffiliatePayoutsQuery(long affiliateId, long botId, int offset, int limit)
    {
        AffiliateId = affiliateId;
        BotId = botId;
        Offset = offset;
        Limit = limit;
    }
}

public class GetAffiliateLinksQuery : IQuery<List<AffiliateLink>>
{
    public long AffiliateId { get; set; }
    public long BotId { get; set; }
    
    public GetAffiliateLinksQuery(long affiliateId, long botId)
    {
        AffiliateId = affiliateId;
        BotId = botId;
    }
}

public class GetChannelQuery : IQuery<ChannelReadModel>
{
    public long ChannelId { get; set; }
    
    public GetChannelQuery(long channelId)
    {
        ChannelId = channelId;
    }
}

public class GetUsersByIdsQuery : IQuery<List<UserReadModel>>
{
    public List<long> UserIds { get; set; }
    
    public GetUsersByIdsQuery(List<long> userIds)
    {
        UserIds = userIds;
    }
}

public class GetPhotosByIdsQuery : IQuery<List<PhotoReadModel>>
{
    public List<long> PhotoIds { get; set; }
    
    public GetPhotosByIdsQuery(List<long> photoIds)
    {
        PhotoIds = photoIds;
    }
}

public class GetContactListBySelfUserIdQuery : IQuery<List<ContactReadModel>>
{
    public long SelfUserId { get; set; }
    
    public GetContactListBySelfUserIdQuery(long selfUserId)
    {
        SelfUserId = selfUserId;
    }
}

public class GetChannelsByIdsQuery : IQuery<List<ChannelReadModel>>
{
    public List<long> ChannelIds { get; set; }
    
    public GetChannelsByIdsQuery(List<long> channelIds)
    {
        ChannelIds = channelIds;
    }
}

public class GetChannelMemberByUserIdListQuery : IQuery<List<ChannelParticipantReadModel>>
{
    public long ChannelId { get; set; }
    public List<long> UserIds { get; set; }
    
    public GetChannelMemberByUserIdListQuery(long channelId, List<long> userIds)
    {
        ChannelId = channelId;
        UserIds = userIds;
    }
}

// Missing types
// TMessageEntity types are defined in MyTelegram.Schema

public enum ChannelType
{
    Unknown,
    Channel,
    Supergroup,
    Gigagroup
}

// Additional missing query classes
public class GetSuggestedPostQuery : IQuery<SuggestedPost>
{
    public string PostId { get; set; }
    
    public GetSuggestedPostQuery(string postId)
    {
        PostId = postId;
    }
}

public class GetChannelSuggestedPostsQuery : IQuery<List<SuggestedPost>>
{
    public long ChannelId { get; set; }
    public int Offset { get; set; }
    public int Limit { get; set; }
    
    public GetChannelSuggestedPostsQuery(long channelId, int offset = 0, int limit = 50)
    {
        ChannelId = channelId;
        Offset = offset;
        Limit = limit;
    }
}

public class GetUserSuggestedPostsQuery : IQuery<List<SuggestedPost>>
{
    public long UserId { get; set; }
    public int Offset { get; set; }
    public int Limit { get; set; }
    
    public GetUserSuggestedPostsQuery(long userId, int offset = 0, int limit = 50)
    {
        UserId = userId;
        Offset = offset;
        Limit = limit;
    }
}

public class GetDirectMessageTopicQuery : IQuery<DirectMessagesTopic>
{
    public string TopicId { get; set; }
    
    public GetDirectMessageTopicQuery(string topicId)
    {
        TopicId = topicId;
    }
}

public class GetChannelDirectMessageTopicsQuery : IQuery<List<DirectMessagesTopic>>
{
    public long ChannelId { get; set; }
    public int Offset { get; set; }
    public int Limit { get; set; }
    
    public GetChannelDirectMessageTopicsQuery(long channelId, int offset = 0, int limit = 50)
    {
        ChannelId = channelId;
        Offset = offset;
        Limit = limit;
    }
}

public class GetUserDirectMessageTopicsQuery : IQuery<List<DirectMessagesTopic>>
{
    public long UserId { get; set; }
    public int Offset { get; set; }
    public int Limit { get; set; }
    
    public GetUserDirectMessageTopicsQuery(long userId, int offset = 0, int limit = 50)
    {
        UserId = userId;
        Offset = offset;
        Limit = limit;
    }
}

public class GetGigagroupAdminsQuery : IQuery<List<GigagroupAdmin>>
{
    public long ChannelId { get; set; }
    
    public GetGigagroupAdminsQuery(long channelId)
    {
        ChannelId = channelId;
    }
}

public class GetGigagroupModeratorsQuery : IQuery<List<GigagroupModerator>>
{
    public long ChannelId { get; set; }
    
    public GetGigagroupModeratorsQuery(long channelId)
    {
        ChannelId = channelId;
    }
}

public class GetGigagroupStatisticsQuery : IQuery<GigagroupStatistics>
{
    public long ChannelId { get; set; }
    public DateTime? From { get; set; }
    public DateTime? To { get; set; }
    
    public GetGigagroupStatisticsQuery(long channelId, DateTime? from = null, DateTime? to = null)
    {
        ChannelId = channelId;
        From = from;
        To = to;
    }
}

public class GetGigagroupModerationHistoryQuery : IQuery<List<GigagroupModerationAction>>
{
    public long ChannelId { get; set; }
    public int Offset { get; set; }
    public int Limit { get; set; }
    
    public GetGigagroupModerationHistoryQuery(long channelId, int offset = 0, int limit = 50)
    {
        ChannelId = channelId;
        Offset = offset;
        Limit = limit;
    }
}

public class GetMonoforumQuery : IQuery<Monoforum>
{
    public long ChannelId { get; set; }
    
    public GetMonoforumQuery(long channelId)
    {
        ChannelId = channelId;
    }
}

public class GetMonoforumTopicQuery : IQuery<MonoforumTopic>
{
    public string TopicId { get; set; }
    
    public GetMonoforumTopicQuery(string topicId)
    {
        TopicId = topicId;
    }
}

public class GetMonoforumTopicsQuery : IQuery<List<MonoforumTopic>>
{
    public long ChannelId { get; set; }
    public int Offset { get; set; }
    public int Limit { get; set; }
    
    public GetMonoforumTopicsQuery(long channelId, int offset = 0, int limit = 50)
    {
        ChannelId = channelId;
        Offset = offset;
        Limit = limit;
    }
}

public class GetMonoforumTopicsByUserQuery : IQuery<List<MonoforumTopic>>
{
    public long UserId { get; set; }
    public int Offset { get; set; }
    public int Limit { get; set; }
    
    public GetMonoforumTopicsByUserQuery(long userId, int offset = 0, int limit = 50)
    {
        UserId = userId;
        Offset = offset;
        Limit = limit;
    }
}

public class GetMonoforumParticipantQuery : IQuery<MonoforumParticipant>
{
    public string TopicId { get; set; }
    public long UserId { get; set; }
    
    public GetMonoforumParticipantQuery(string topicId, long userId)
    {
        TopicId = topicId;
        UserId = userId;
    }
}

public class GetMonoforumMessagesQuery : IQuery<List<MonoforumMessage>>
{
    public string TopicId { get; set; }
    public int Offset { get; set; }
    public int Limit { get; set; }
    
    public GetMonoforumMessagesQuery(string topicId, int offset = 0, int limit = 50)
    {
        TopicId = topicId;
        Offset = offset;
        Limit = limit;
    }
}

public class GetMonoforumMessageQuery : IQuery<MonoforumMessage>
{
    public string MessageId { get; set; }
    
    public GetMonoforumMessageQuery(string messageId)
    {
        MessageId = messageId;
    }
}

public class GetMonoforumStatisticsQuery : IQuery<MonoforumStatistics>
{
    public long ChannelId { get; set; }
    public DateTime? From { get; set; }
    public DateTime? To { get; set; }
    
    public GetMonoforumStatisticsQuery(long channelId, DateTime? from = null, DateTime? to = null)
    {
        ChannelId = channelId;
        From = from;
        To = to;
    }
}

public class GetMonoforumTopicStatisticsQuery : IQuery<MonoforumTopicStatistics>
{
    public string TopicId { get; set; }
    
    public GetMonoforumTopicStatisticsQuery(string topicId)
    {
        TopicId = topicId;
    }
}

public class GetMonoforumModerationHistoryQuery : IQuery<List<MonoforumModerationAction>>
{
    public long ChannelId { get; set; }
    public int Offset { get; set; }
    public int Limit { get; set; }
    
    public GetMonoforumModerationHistoryQuery(long channelId, int offset = 0, int limit = 50)
    {
        ChannelId = channelId;
        Offset = offset;
        Limit = limit;
    }
}

public class GetPaidMediaPurchaseQuery : IQuery<PaidMediaPurchase>
{
    public string PurchaseId { get; set; }
    
    public GetPaidMediaPurchaseQuery(string purchaseId)
    {
        PurchaseId = purchaseId;
    }
}

public class GetUserPaidMediaPurchasesQuery : IQuery<List<PaidMediaPurchase>>
{
    public long UserId { get; set; }
    public int Offset { get; set; }
    public int Limit { get; set; }
    
    public GetUserPaidMediaPurchasesQuery(long userId, int offset = 0, int limit = 50)
    {
        UserId = userId;
        Offset = offset;
        Limit = limit;
    }
}

public class GetPaidMediaStatsQuery : IQuery<PaidMediaStatistics>
{
    public long ChannelId { get; set; }
    public DateTime? From { get; set; }
    public DateTime? To { get; set; }
    
    public GetPaidMediaStatsQuery(long channelId, DateTime? from = null, DateTime? to = null)
    {
        ChannelId = channelId;
        From = from;
        To = to;
    }
}

public class GetPaidMediaPurchaseByIdQuery : IQuery<PaidMediaPurchase>
{
    public string PurchaseId { get; set; }
    
    public GetPaidMediaPurchaseByIdQuery(string purchaseId)
    {
        PurchaseId = purchaseId;
    }
}

public class GetExistingDirectMessageTopicQuery : IQuery<DirectMessagesTopic>
{
    public long ChannelId { get; set; }
    public long UserId { get; set; }
    
    public GetExistingDirectMessageTopicQuery(long channelId, long userId)
    {
        ChannelId = channelId;
        UserId = userId;
    }
}

public class GetDirectMessageStatisticsQuery : IQuery<DirectMessageStatistics>
{
    public long ChannelId { get; set; }
    public DateTime? From { get; set; }
    public DateTime? To { get; set; }
    
    public GetDirectMessageStatisticsQuery(long channelId, DateTime? from = null, DateTime? to = null)
    {
        ChannelId = channelId;
        From = from;
        To = to;
    }
}

public class GetUserStarsRatingQuery : IQuery<StarsRating>
{
    public long UserId { get; set; }
    
    public GetUserStarsRatingQuery(long userId)
    {
        UserId = userId;
    }
}

public class GetStarsLeaderboardQuery : IQuery<List<StarsLeaderboardEntry>>
{
    public LeaderboardType Type { get; set; }
    public LeaderboardPeriod Period { get; set; }
    public int Limit { get; set; }
    
    public GetStarsLeaderboardQuery(LeaderboardType type, LeaderboardPeriod period, int limit = 100)
    {
        Type = type;
        Period = period;
        Limit = limit;
    }
}

public class GetTopUsersQuery : IQuery<List<StarsUserRanking>>
{
    public LeaderboardType Type { get; set; }
    public LeaderboardPeriod Period { get; set; }
    public int Limit { get; set; }
    
    public GetTopUsersQuery(LeaderboardType type, LeaderboardPeriod period, int limit = 100)
    {
        Type = type;
        Period = period;
        Limit = limit;
    }
}

public class GetUserStarsAchievementsQuery : IQuery<List<StarsAchievement>>
{
    public long UserId { get; set; }
    
    public GetUserStarsAchievementsQuery(long userId)
    {
        UserId = userId;
    }
}

public class GetUserStarsStatisticsQuery : IQuery<StarsStatistics>
{
    public long UserId { get; set; }
    public DateTime? From { get; set; }
    public DateTime? To { get; set; }
    
    public GetUserStarsStatisticsQuery(long userId, DateTime? from = null, DateTime? to = null)
    {
        UserId = userId;
        From = from;
        To = to;
    }
}

public class GetUserStarsActivityQuery : IQuery<List<StarsActivity>>
{
    public long UserId { get; set; }
    public DateTime? From { get; set; }
    public DateTime? To { get; set; }
    public int Limit { get; set; }
    
    public GetUserStarsActivityQuery(long userId, DateTime? from = null, DateTime? to = null, int limit = 100)
    {
        UserId = userId;
        From = from;
        To = to;
        Limit = limit;
    }
}

public class GetStarsInPeriodQuery : IQuery<List<StarsTransaction>>
{
    public long UserId { get; set; }
    public DateTime From { get; set; }
    public DateTime To { get; set; }
    
    public GetStarsInPeriodQuery(long userId, DateTime from, DateTime to)
    {
        UserId = userId;
        From = from;
        To = to;
    }
}

// Additional missing query classes with correct constructors
public class GetPaidMediaPricingQuery : IQuery<PaidMediaPricing>
{
    public long ChannelId { get; set; }
    
    public GetPaidMediaPricingQuery(long channelId)
    {
        ChannelId = channelId;
    }
}

// Fixed GetChannelSuggestedPostsQuery with status parameter
public class GetChannelSuggestedPostsQueryWithStatus : IQuery<List<SuggestedPost>>
{
    public long ChannelId { get; set; }
    public SuggestedPostStatus? Status { get; set; }
    public int Offset { get; set; }
    public int Limit { get; set; }
    
    public GetChannelSuggestedPostsQueryWithStatus(long channelId, SuggestedPostStatus? status, int offset = 0, int limit = 50)
    {
        ChannelId = channelId;
        Status = status;
        Offset = offset;
        Limit = limit;
    }
}

// Additional missing queries
public class GetReferralProgramByBotIdQuery : IQuery<StarReferralProgram>
{
    public long BotId { get; set; }
    
    public GetReferralProgramByBotIdQuery(long botId)
    {
        BotId = botId;
    }
}

public class GetPaidMessageSettingsQuery : IQuery<PaidMessageSettings>
{
    public long ChannelId { get; set; }
    
    public GetPaidMessageSettingsQuery(long channelId)
    {
        ChannelId = channelId;
    }
}

public class GetPaidMessageRevenueQuery : IQuery<PaidMessageRevenue>
{
    public long ChannelId { get; set; }
    
    public GetPaidMessageRevenueQuery(long channelId)
    {
        ChannelId = channelId;
    }
}

public class GetDirectMessageSettingsQuery : IQuery<DirectMessageSettings>
{
    public long ChannelId { get; set; }
    
    public GetDirectMessageSettingsQuery(long channelId)
    {
        ChannelId = channelId;
    }
}

public class GetMonoforumSettingsQuery : IQuery<MonoforumSettings>
{
    public long ChannelId { get; set; }
    
    public GetMonoforumSettingsQuery(long channelId)
    {
        ChannelId = channelId;
    }
}

public class GetGigagroupSettingsQuery : IQuery<GigagroupSettings>
{
    public long ChannelId { get; set; }
    
    public GetGigagroupSettingsQuery(long channelId)
    {
        ChannelId = channelId;
    }
}

public class GetAffiliateQuery : IQuery<Affiliate>
{
    public long AffiliateId { get; set; }
    public long BotId { get; set; }
    
    public GetAffiliateQuery(long affiliateId, long botId)
    {
        AffiliateId = affiliateId;
        BotId = botId;
    }
}

public class GetReferralProgramQuery : IQuery<StarReferralProgram>
{
    public string ProgramId { get; set; }
    
    public GetReferralProgramQuery(string programId)
    {
        ProgramId = programId;
    }
}

// Stars balance queries
public class GetStarsBalanceQuery : IQuery<StarsStatus>
{
    public long PeerId { get; set; }
    
    public GetStarsBalanceQuery(long peerId)
    {
        PeerId = peerId;
    }
}

public class GetStarsStatusQueryOld : IQuery<ServicesStarsStatus>
{
    public long PeerId { get; set; }
    public bool IsTon { get; set; }
    
    public GetStarsStatusQueryOld(long peerId, bool isTon = false)
    {
        PeerId = peerId;
        IsTon = isTon;
    }
}

public class GetStarsTransactionsQueryOld : IQuery<ServicesStarsStatus>
{
    public long PeerId { get; set; }
    public bool IsInbound { get; set; }
    public bool IsOutbound { get; set; }
    public bool IsAscending { get; set; }
    public bool IsTon { get; set; }
    public string? SubscriptionId { get; set; }
    public string Offset { get; set; } = string.Empty;
    public int Limit { get; set; }
    
    public GetStarsTransactionsQueryOld()
    {
        Limit = 50;
    }
}

public class GetStarsRevenueStatsQuery : IQuery<StarsRevenueStats>
{
    public long PeerId { get; set; }
    public bool IsDarkTheme { get; set; }
    public bool IsTon { get; set; }
    
    public GetStarsRevenueStatsQuery(long peerId, bool isDarkTheme = false, bool isTon = false)
    {
        PeerId = peerId;
        IsDarkTheme = isDarkTheme;
        IsTon = isTon;
    }
}

// Additional privacy settings
public class PeerSettings
{
    public bool CanSendMessages { get; set; } = true;
    public bool CanSendMedia { get; set; } = true;
    public bool CanSendStickers { get; set; } = true;
    public bool CanSendGifs { get; set; } = true;
    public bool CanSendGames { get; set; } = true;
    public bool CanSendInline { get; set; } = true;
}

public class GlobalPrivacySettings
{
    public bool ArchiveAndMuteNewNoncontactPeers { get; set; }
    public bool KeepArchivedUnmuted { get; set; }
    public bool KeepArchivedFolders { get; set; }
    public bool HideReadTime { get; set; }
}

// Additional missing query classes
public class GetQuickRepliesQuery : IQuery<QuickReplies>
{
    public long UserId { get; set; }
    
    public GetQuickRepliesQuery(long userId)
    {
        UserId = userId;
    }
}

public class GetQuickReplyMessagesQuery : IQuery<List<QuickReplyMessage>>
{
    public long UserId { get; set; }
    public int ShortcutId { get; set; }
    public List<int> MessageIds { get; set; } = new();
    
    public GetQuickReplyMessagesQuery(long userId, int shortcutId, List<int> messageIds)
    {
        UserId = userId;
        ShortcutId = shortcutId;
        MessageIds = messageIds;
    }
}

public class QuickReplyMessage
{
    public int Id { get; set; }
    public string Message { get; set; } = string.Empty;
    public List<MyTelegram.Domain.Shared.Business.MessageEntity> Entities { get; set; } = new();
    public DateTime Date { get; set; }
    public MyTelegram.Schema.IMessageMedia? Media { get; set; }
}

public class QuickReplyShortcut
{
    public int ShortcutId { get; set; }
    public string Shortcut { get; set; } = string.Empty;
    public List<QuickReplyMessage> Messages { get; set; } = new();
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class ConnectedBot
{
    public long BotUserId { get; set; }
    public bool CanReply { get; set; }
    public BusinessBotRecipients Recipients { get; set; } = new();
    public BusinessBotRights Rights { get; set; } = new();
}

public class BusinessBotRecipients
{
    public bool ExistingChats { get; set; }
    public bool NewChats { get; set; }
    public bool Contacts { get; set; }
    public bool NonContacts { get; set; }
    public bool ExcludeSelected { get; set; }
    public List<long> Users { get; set; } = new();
}

public class BusinessBotRights
{
    public bool CanManageTopics { get; set; }
    public bool CanEditInviteLinks { get; set; }
    public bool CanDeleteMessages { get; set; }
    public bool CanManageGroups { get; set; }
    public bool CanInviteUsers { get; set; }
    public bool CanPinMessages { get; set; }
    public bool CanManageTopics2 { get; set; }
    public bool CanManageStickerSets { get; set; }
    public bool CanWriteSendToChannel { get; set; }
    public bool CanPostMessages { get; set; }
    public bool CanEditMessages { get; set; }
    public bool CanPromoteMembers { get; set; }
}

public class Bot
{
    public long Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public bool IsVerified { get; set; }
    public bool IsPremium { get; set; }
}

public class User
{
    public long Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public bool IsBot { get; set; }
    public bool IsVerified { get; set; }
    public bool IsPremium { get; set; }
}

// Additional types needed by PaidMediaAppService
public record PaidMediaUnlockRequest(long UserId, string PaidMediaId);
public record PaidMediaUnlockResult(bool Success, PaidMedia? Media);
public record PaidMediaStats(long TotalPurchases, long TotalRevenue, DateTime PeriodStart, DateTime PeriodEnd);

public class GetChecklistQuery : IQuery<Checklist>
{
    public string ChecklistId { get; set; }
    
    public GetChecklistQuery(string checklistId)
    {
        ChecklistId = checklistId;
    }
}

public class GetConnectedBotsQueryOld : IQuery<List<Bot>>
{
    public long UserId { get; set; }
    
    public GetConnectedBotsQueryOld(long userId)
    {
        UserId = userId;
    }
}

public class GetUserListByIdQuery : IQuery<List<User>>
{
    public List<long> UserIds { get; set; }
    
    public GetUserListByIdQuery(List<long> userIds)
    {
        UserIds = userIds;
    }
}

public class GetConnectedBotsResult
{
    public List<ConnectedBot> ConnectedBots { get; set; } = new();
}

public class GetUserListByIdResult
{
    public List<User> Users { get; set; } = new();
}

public class GetQuickRepliesResult
{
    public List<QuickReply> QuickReplies { get; set; } = new();
    public List<QuickReplyShortcut> QuickReplyShortcuts { get; set; } = new();
}

public class GetPaidMediaQuery : IQuery<PaidMedia>
{
    public string PaidMediaId { get; set; }
    
    public GetPaidMediaQuery(string paidMediaId)
    {
        PaidMediaId = paidMediaId;
    }
}

// Stars queries
public class GetAllLevelsQuery : IQuery<List<StarsLevel>>
{
}

// Mock types for compilation
public class GetPeerNotifySettingsByIdQuery : IQuery<PeerNotifySettingsReadModel>
{
    public string Id { get; set; }
    
    public GetPeerNotifySettingsByIdQuery(string id)
    {
        Id = id;
    }
}

public class PeerNotifySettingsReadModel
{
    public NotifySettings NotifySettings { get; set; }
}

public class NotifySettings
{
    public static NotifySettings DefaultSettings = new NotifySettings();
}

// MessageEntity is defined in MyTelegram.Domain.Shared.Business

// Missing types that were referenced in queries
public class MonoforumParticipant
{
    public string TopicId { get; set; }
    public long UserId { get; set; }
    public MonoforumParticipantRole Role { get; set; }
    public bool IsAnonymous { get; set; }
    public bool CanPost { get; set; }
    public DateTime JoinedAt { get; set; }
}

public class MonoforumMessage
{
    public string Id { get; set; }
    public string TopicId { get; set; }
    public long SenderId { get; set; }
    public string Content { get; set; }
    public MonoforumMessageType Type { get; set; }
    public DateTime SentAt { get; set; }
    public bool IsAnonymous { get; set; }
    public MonoforumMessageStatus Status { get; set; }
}

public class MonoforumStatistics
{
    public long TotalTopics { get; set; }
    public long ActiveTopics { get; set; }
    public long TotalMessages { get; set; }
    public long ActiveParticipants { get; set; }
    public DateTime From { get; set; }
    public DateTime To { get; set; }
}

public class MonoforumTopicStatistics
{
    public string TopicId { get; set; }
    public long TotalMessages { get; set; }
    public long ActiveParticipants { get; set; }
    public DateTime LastActivity { get; set; }
}

public class MonoforumModerationAction
{
    public string Id { get; set; }
    public long ChannelId { get; set; }
    public long ModeratorId { get; set; }
    public MonoforumModerationActionType Action { get; set; }
    public long? TargetUserId { get; set; }
    public string? MessageId { get; set; }
    public string Reason { get; set; }
    public DateTime PerformedAt { get; set; }
}

public class PaidMediaPurchase
{
    public string Id { get; set; }
    public long UserId { get; set; }
    public long ChannelId { get; set; }
    public string PaidMediaId { get; set; }
    public long StarsAmount { get; set; }
    public string TransactionId { get; set; }
    public DateTime PurchasedAt { get; set; }
}

public class PaidMediaStatistics
{
    public long TotalPurchases { get; set; }
    public long TotalStarsEarned { get; set; }
    public decimal Revenue { get; set; }
    public DateTime From { get; set; }
    public DateTime To { get; set; }
}

public class DirectMessagesTopic
{
    public string Id { get; set; }
    public long ChannelId { get; set; }
    public long UserId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public DirectMessageTopicStatus Status { get; set; }
    public bool IsActive { get; set; }
    public DirectMessageSettings Settings { get; set; }
}

// Enum types
public enum MonoforumParticipantRole
{
    Member,
    Moderator,
    Admin,
    Creator
}

public enum MonoforumMessageType
{
    Text,
    Media,
    Poll,
    Document
}

public enum MonoforumMessageStatus
{
    Pending,
    Approved,
    Rejected,
    Deleted
}

public enum MonoforumModerationActionType
{
    DeleteMessage,
    KickUser,
    BanUser,
    ApproveMessage,
    RejectMessage
}

public enum DirectMessageTopicStatus
{
    Active,
    Closed,
    Archived,
    Banned
}

// Leaderboard types
public enum LeaderboardType
{
    Global,
    Regional,
    Country,
    City,
    Channel,
    AgeGroup,
    Interest
}

public enum LeaderboardPeriod
{
    Daily,
    Weekly,
    Monthly,
    Yearly,
    AllTime
}

// Stars related types
public class StarsRevenueStats
{
    public long TotalRevenue { get; set; }
    public long DailyRevenue { get; set; }
    public long WeeklyRevenue { get; set; }
    public long MonthlyRevenue { get; set; }
    public DateTime From { get; set; }
    public DateTime To { get; set; }
    public string Status { get; set; } = string.Empty;
}

public class StarsLeaderboardEntry
{
    public long UserId { get; set; }
    public long TotalStars { get; set; }
    public int Position { get; set; }
    public DateTime LastUpdated { get; set; }
}

public class StarsUserRanking
{
    public long UserId { get; set; }
    public string Username { get; set; }
    public long TotalStars { get; set; }
    public int Position { get; set; }
    public int PreviousPosition { get; set; }
}

public class StarsAchievement
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string Icon { get; set; }
    public int Points { get; set; }
    public DateTime EarnedAt { get; set; }
}

public class StarsStatistics
{
    public long UserId { get; set; }
    public long TotalStarsSpent { get; set; }
    public long TotalStarsReceived { get; set; }
    public decimal AverageTransactionSize { get; set; }
    public int GiftsSent { get; set; }
    public int GiftsReceived { get; set; }
    public DateTime From { get; set; }
    public DateTime To { get; set; }
    public List<StarsActivity> Activities { get; set; } = new();
    public int TotalTransactions { get; set; }
    public int ActiveDaysCount { get; set; }
    public int PostsSuggested { get; set; }
}

public class StarsActivity
{
    public string Id { get; set; }
    public long UserId { get; set; }
    public StarsActivityType Type { get; set; }
    public long Amount { get; set; }
    public DateTime CreatedAt { get; set; }
    public string Description { get; set; }
}

public enum StarsActivityType
{
    GiftSent,
    GiftReceived,
    MessageSent,
    PostSuggested,
    MessageBoosted,
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

// Additional required types
public class DirectMessageSettings
{
    public bool Enabled { get; set; }
    public long BasePrice { get; set; }
    public bool RequireApproval { get; set; }
    public bool AllowAnonymous { get; set; }
}

public class DirectMessageStatistics
{
    public long TotalMessages { get; set; }
    public long TotalRevenue { get; set; }
    public long ActiveTopics { get; set; }
    public DateTime From { get; set; }
    public DateTime To { get; set; }
}

// Base class fix
public abstract class ValueObject
{
    protected abstract IEnumerable<object> GetEqualityComponents();
    
    public override bool Equals(object obj)
    {
        if (obj == null || obj.GetType() != GetType())
        {
            return false;
        }

        var other = (ValueObject)obj;
        return GetEqualityComponents().SequenceEqual(other.GetEqualityComponents());
    }

    public override int GetHashCode()
    {
        return GetEqualityComponents()
            .Aggregate(1, (current, obj) => current * 23 + (obj?.GetHashCode() ?? 0));
    }
}

// Additional missing types
public class MonoforumSettings
{
    public bool AllowPublicTopics { get; set; } = true;
    public bool RequireVerification { get; set; } = false;
    public bool AutoModerateMessages { get; set; } = true;
    public bool FilterProfanity { get; set; } = true;
    public bool FilterSpam { get; set; } = true;
    public bool EnableSlowMode { get; set; } = false;
    public bool RequireAccountAge { get; set; } = true;
    public int MinAccountAgeDays { get; set; } = 7;
    public bool RequirePhoneNumber { get; set; } = true;
}

public class PaidMediaPricing
{
    public long BasePrice { get; set; }
    public long ExtendedPrice { get; set; }
    public string Currency { get; set; } = "stars";
    public DateTime UpdatedAt { get; set; }
}

public class GigagroupSettings
{
    public bool EnableJoinRequests { get; set; } = true;
    public bool EnableRestrictedInvites { get; set; } = true;
    public bool EnableAdminApprovalForJoinRequests { get; set; } = true;
    public bool AllowMedia { get; set; } = true;
    public bool AllowLinks { get; set; } = false;
    public bool AllowForwards { get; set; } = false;
    public bool EnableStatistics { get; set; } = true;
    public bool EnableContentModeration { get; set; } = true;
}

public class GigagroupStatistics
{
    public long TotalMembers { get; set; }
    public long ActiveMembers { get; set; }
    public long TotalMessages { get; set; }
    public long JoinRequests { get; set; }
    public DateTime From { get; set; }
    public DateTime To { get; set; }
}

public class GigagroupModerationAction
{
    public string Id { get; set; }
    public long ChannelId { get; set; }
    public long ModeratorId { get; set; }
    public GigagroupModerationActionType Action { get; set; }
    public long? TargetUserId { get; set; }
    public string? MessageId { get; set; }
    public string Reason { get; set; }
    public DateTime PerformedAt { get; set; }
}

public enum GigagroupModerationActionType
{
    DeleteMessage,
    KickUser,
    BanUser,
    ApproveJoinRequest,
    RejectJoinRequest
}

public class StarsLevel
{
    public int Level { get; set; }
    public string Title { get; set; }
    public long RequiredStars { get; set; }
    public string Color { get; set; }
    public string Icon { get; set; }
    public List<string> Benefits { get; set; } = new();
    public bool IsSpecial { get; set; } = false;
    public string Badge { get; set; }
    public decimal TrustMultiplier { get; set; } = 1.0m;
}

public class AffiliatePayout
{
    public string Id { get; set; }
    public long AffiliateId { get; set; }
    public long BotId { get; set; }
    public long Amount { get; set; }
    public DateTime RequestedAt { get; set; }
    public PayoutStatus Status { get; set; }
    public DateTime? ProcessedAt { get; set; }
}

public class AffiliateLink
{
    public string Id { get; set; }
    public long AffiliateId { get; set; }
    public long BotId { get; set; }
    public string Url { get; set; }
    public string Campaign { get; set; }
    public string Description { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ExpiresAt { get; set; }
    public int Clicks { get; set; }
    public int Conversions { get; set; }
}

public class Monoforum
{
    public string Id { get; set; }
    public long ChannelId { get; set; }
    public long CreatorId { get; set; }
    public bool IsMonoforum { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public MonoforumSettings Settings { get; set; }
}

public class MonoforumTopic
{
    public string Id { get; set; }
    public long ChannelId { get; set; }
    public long CreatorId { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public MonoforumTopicStatus Status { get; set; }
    public bool IsPublic { get; set; }
    public bool IsAnonymous { get; set; }
    public List<string> Tags { get; set; } = new();
    public bool RequiresApproval { get; set; }
}

public enum MonoforumTopicStatus
{
    Active,
    Closed,
    Archived,
    Pending,
    Rejected
}

public enum PayoutStatus
{
    Pending,
    Approved,
    Rejected,
    Processed
}

// Fix InputPrivacyKeyAddParticipants to properly implement IInputPrivacyKey
public class InputPrivacyKeyAddParticipants : IInputPrivacyKey
{
    public uint ConstructorId => 0x4D95AB94; // Some valid constructor ID

    public void Serialize(IBufferWriter<byte> writer)
    {
        // Implementation needed
    }

    public void Deserialize(ref ReadOnlyMemory<byte> data)
    {
        // Implementation needed
    }
}

// Fix PeerNotifySettingsId
public class PeerNotifySettingsId : ValueObject
{
    public string Value { get; }
    
    public PeerNotifySettingsId(string value)
    {
        Value = value;
    }
    
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
    
    public static PeerNotifySettingsId Create(long userId, PeerType peerType, long peerId)
    {
        return new PeerNotifySettingsId($"{userId}_{(int)peerType}_{peerId}");
    }
}

public enum PeerType
{
    User,
    Chat,
    Channel
}

// Additional missing types
public class ChannelParticipantReadModel
{
    public long ChannelId { get; set; }
    public long UserId { get; set; }
    public ChannelParticipantRole Role { get; set; }
    public DateTime JoinedAt { get; set; }
    public bool IsAdmin { get; set; }
    public RequestInfo RequestInfo { get; set; }
    public long TargetUserId { get; set; }
    public MyTelegram.Messenger.Services.Impl.PeerSettings Settings { get; set; }
}

public enum ChannelParticipantRole
{
    Member,
    Moderator,
    Admin,
    Creator
}

// Additional missing types
public class AffiliateCommission
{
    public string Id { get; set; }
    public long AffiliateId { get; set; }
    public string ReferralId { get; set; }
    public long BotId { get; set; }
    public long Amount { get; set; }
    public int CommissionPermille { get; set; }
    public DateTime CreatedAt { get; set; }
    public string Description { get; set; }
    public string? PurchaseId { get; set; }
    public long OriginalAmount { get; set; }
}

public class SuggestedPost
{
    public string Id { get; set; }
    public long ChannelId { get; set; }
    public long UserId { get; set; }
    public string Content { get; set; }
    public SuggestedPostStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public long? StarsAmount { get; set; }
    public bool IsPremium { get; set; }
    public List<string> Tags { get; set; } = new();
}

public enum SuggestedPostStatus
{
    Pending,
    Approved,
    Rejected,
    Published,
    Archived
}

public class Checklist
{
    public string Id { get; set; }
    public string MessageId { get; set; }
    public long SenderId { get; set; }
    public long ChannelId { get; set; }
    public long PeerId { get; set; }
    public string Title { get; set; }
    public List<ChecklistTask> Tasks { get; set; } = new();
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public ChecklistStatus Status { get; set; }
}

public class ChecklistTask
{
    public string Id { get; set; }
    public string Text { get; set; }
    public bool IsCompleted { get; set; }
    public bool IsMandatory { get; set; }
    public int Position { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<string> Entities { get; set; } = new();
}

public enum ChecklistStatus
{
    Active,
    Completed,
    Archived
}

public class PaidMedia
{
    public string Id { get; set; }
    public long ChannelId { get; set; }
    public long CreatorId { get; set; }
    public PaidMediaType Type { get; set; }
    public long StarsAmount { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public string Thumbnail { get; set; }
    public List<PaidMediaItem> MediaItems { get; set; } = new();
    public DateTime CreatedAt { get; set; }
    public bool IsActive { get; set; }
}

public class PaidMediaItem
{
    public string Id { get; set; }
    public string Url { get; set; }
    public string Type { get; set; }
    public string? Preview { get; set; }
    public long Size { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }
    public string? Caption { get; set; }
}

public enum PaidMediaType
{
    Photo,
    Video,
    Document,
    Audio
}

public class ServicesStarsTransaction
{
    public string Id { get; set; }
    public long UserId { get; set; }
    public long Amount { get; set; }
    public StarsTransactionType Type { get; set; }
    public DateTime CreatedAt { get; set; }
    public string Description { get; set; }
    public string? SourceId { get; set; }
    public bool IsRefund { get; set; }
}

public enum StarsTransactionType
{
    Purchase,
    Gift,
    Refund,
    Bonus,
    Penalty
}

// Fix PeerSettingsId
public class PeerSettingsId : ValueObject
{
    public string Value { get; }
    
    public PeerSettingsId(string value)
    {
        Value = value;
    }
    
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
    
    public static PeerSettingsId Create(long userId, long peerId)
    {
        return new PeerSettingsId($"{userId}_{peerId}");
    }
}

// Additional missing types
public class Referral
{
    public string Id { get; set; }
    public string AffiliateId { get; set; }
    public long ReferredUserId { get; set; }
    public long BotId { get; set; }
    public string ReferralCode { get; set; }
    public ReferralStatus Status { get; set; }
    public DateTime ReferredAt { get; set; }
    public string? Source { get; set; }
    public string? Campaign { get; set; }
}

public enum ReferralStatus
{
    Pending,
    Approved,
    Rejected,
    Expired
}

public class StarsRating
{
    public string Id { get; set; }
    public long UserId { get; set; }
    public int Level { get; set; }
    public long TotalStars { get; set; }
    public int Position { get; set; }
    public string Title { get; set; }
    public string Color { get; set; }
    public string Icon { get; set; }
    public long TotalStarsSpent { get; set; }
    public long TotalStarsReceived { get; set; }
    public int StarsLevel { get; set; }
    public string LevelTitle { get; set; }
    public double TrustScore { get; set; }
    public DateTime LastUpdated { get; set; }
    public bool IsVerified { get; set; }
    public bool IsPremium { get; set; }
    public DateTime UpdatedAt { get; set; }
    public long StarsToNextLevel { get; set; }
    public MyTelegram.Domain.Shared.StarsRating.RankPosition GlobalRank { get; set; } = new();
}

public class ServicesStarsStatus
{
    public long UserId { get; set; }
    public long Balance { get; set; }
    public long TotalEarned { get; set; }
    public long TotalSpent { get; set; }
    public DateTime LastUpdated { get; set; }
    public List<ServicesStarsTransaction> RecentTransactions { get; set; } = new();
    public List<ServicesStarsSubscription> Subscriptions { get; set; } = new();
    public List<ServicesStarsTransaction> History { get; set; } = new();
}

public class ServicesStarsSubscription
{
    public string Id { get; set; } = string.Empty;
    public long UserId { get; set; }
    public long PeerId { get; set; }
    public int StarsAmount { get; set; }
    public DateTime StartedAt { get; set; }
    public DateTime? ExpiresAt { get; set; }
    public bool IsActive { get; set; }
}

// Additional missing types
public class GigagroupAdmin
{
    public string Id { get; set; }
    public long ChannelId { get; set; }
    public long UserId { get; set; }
    public DateTime PromotedAt { get; set; }
    public bool IsActive { get; set; }
    public GigagroupRole Role { get; set; }
}

public class GigagroupModerator
{
    public string Id { get; set; }
    public long ChannelId { get; set; }
    public long UserId { get; set; }
    public DateTime PromotedAt { get; set; }
    public bool IsActive { get; set; }
    public List<string> Permissions { get; set; } = new();
}

public class QuickReplies
{
    public long UserId { get; set; }
    public List<QuickReply> Replies { get; set; } = new();
    public List<QuickReply> QuickReplyList { get; set; } = new();
    public List<QuickReplyShortcut> Shortcuts { get; set; } = new();
    public string Hash { get; set; } = string.Empty;
    public DateTime UpdatedAt { get; set; }
}

// Missing execution result with MessageId
public class SendMessageExecutionResult : IExecutionResult
{
    public bool Success { get; set; }
    public bool IsSuccess => Success;
    public long MessageId { get; set; }
    public string ErrorMessage { get; set; } = string.Empty;
}

// Missing message commands and queries
public class SendMessageCommand : Command<TempAggregate, TempId, SendMessageExecutionResult>
{
    public SendMessageCommand(long userId, RequestInfo requestInfo, long peerId, string message, List<MyTelegram.Domain.Shared.Business.MessageEntity> entities = null, MyTelegram.Schema.IMessageMedia media = null, long replyToMsgId = 0) : base(TempId.With(userId.ToString())) 
    { 
        SenderId = userId;
        RequestInfo = requestInfo;
        PeerId = peerId;
        Message = message;
        Entities = entities ?? new();
        Media = media;
        ReplyToMsgId = replyToMsgId;
    }
    
    public long SenderId { get; set; }
    public RequestInfo RequestInfo { get; set; } = new(0, string.Empty);
    public long PeerId { get; set; }
    public string Message { get; set; } = string.Empty;
    public List<MyTelegram.Domain.Shared.Business.MessageEntity> Entities { get; set; } = new();
    public MyTelegram.Schema.IMessageMedia Media { get; set; }
    public long ReplyToMsgId { get; set; }
}

public class GetMessagesByIdsQuery : IQuery<List<MyTelegram.ReadModel.Interfaces.IMessageReadModel>>
{
    public long UserId { get; set; }
    public List<int> MessageIds { get; set; } = new();
    
    public GetMessagesByIdsQuery(long userId, List<int> messageIds)
    {
        UserId = userId;
        MessageIds = messageIds;
    }
}

public enum GigagroupRole
{
    Admin,
    Moderator,
    Creator
}

// Missing Affiliate class
public class Affiliate
{
    public string Id { get; set; }
    public long AffiliateId { get; set; }
    public long BotId { get; set; }
    public string ReferralCode { get; set; }
    public string CustomReferralUrl { get; set; }
    public int CurrentTier { get; set; }
    public DateTime JoinedAt { get; set; }
    public bool IsActive { get; set; }
    public bool IsVerified { get; set; }
    public List<AffiliateStats> Stats { get; set; } = new();
}

// AffiliateStats is defined in MyTelegram.Domain.Shared.Affiliate

// Additional missing types
public class ChannelParticipant
{
    public string Id { get; set; }
    public long ChannelId { get; set; }
    public long UserId { get; set; }
    public ChannelParticipantRole Role { get; set; }
    public DateTime JoinedAt { get; set; }
    public bool IsAdmin { get; set; }
    public string? CustomTitle { get; set; }
}

// StarReferralProgram is defined in MyTelegram.Domain.Shared.Affiliate

public class UpdatePeerSettingsCommand : Command<PeerSettingsAggregate, MyTelegram.Domain.Aggregates.PeerSetting.PeerSettingsId>
{
    public UpdatePeerSettingsCommand(MyTelegram.Domain.Aggregates.PeerSetting.PeerSettingsId peerSettingsId, RequestInfo requestInfo, long targetUserId, MyTelegram.Messenger.Services.Impl.PeerSettings settings) 
        : base(peerSettingsId)
    {
        RequestInfo = requestInfo;
        TargetUserId = targetUserId;
        Settings = settings;
    }
    
    public RequestInfo RequestInfo { get; set; }
    public long TargetUserId { get; set; }
    public MyTelegram.Messenger.Services.Impl.PeerSettings Settings { get; set; }
}
