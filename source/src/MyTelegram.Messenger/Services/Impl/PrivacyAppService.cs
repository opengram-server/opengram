using MyTelegram.Queries.Privacy;
using MyTelegram.Domain.Commands.Privacy;
using MyTelegram.Domain.Commands.User;
using MyTelegram.Schema;
using MyTelegram.Messenger.Services.Impl;
using MyTelegram.Messenger.Services;
using MyTelegram.Messenger.Services.Interfaces;
using MyTelegram.Domain.Shared;
using MyTelegram.Core;
using Microsoft.Extensions.Logging;
using MyTelegram.Queries;
using DomainGlobalPrivacySettings = MyTelegram.GlobalPrivacySettings;
using GlobalPrivacySettings = MyTelegram.Messenger.Services.Impl.GlobalPrivacySettings;

namespace MyTelegram.Messenger.Services.Impl;

public class PrivacyAppService(
    ICacheManager<GlobalPrivacySettingsCacheItem> cacheManager,
    IQueryProcessor queryProcessor,
    ICommandBus commandBus,
    ILogger<PrivacyAppService> logger)
    : BaseAppService, IPrivacyAppService, ITransientDependency
{
    public async Task<IReadOnlyCollection<IPrivacyReadModel>> GetPrivacyListAsync(IReadOnlyList<long> userIds)
    {
        var privacyReadModels = new List<IPrivacyReadModel>();
        foreach (var userId in userIds)
        {
            var userPrivacy = await queryProcessor.ProcessAsync(new GetUserPrivacyListQuery(userId));
            privacyReadModels.AddRange(userPrivacy);
        }
        return privacyReadModels;
    }

    public async Task<IReadOnlyCollection<IPrivacyReadModel>> GetPrivacyListAsync(long userId)
    {
        return await queryProcessor.ProcessAsync(new GetUserPrivacyListQuery(userId));
    }

    public Task<IReadOnlyCollection<IPrivacyReadModel>> GetPrivacyListAsync(IInputPrivacyKey key, long? phone = null)
    {
        return queryProcessor.ProcessAsync(new GetPrivacyByKeyQuery(key, phone));
    }

    public async Task<TVector<IPrivacyRule>> GetPrivacyRulesAsync(long userId, IInputPrivacyKey key)
    {
        var privacyReadModels = await queryProcessor.ProcessAsync(new GetPrivacyByUserIdQuery(userId, key));
        var privacyValueDataList = privacyReadModels.Select(p => p.PrivacyValueDataList).FirstOrDefault();
        
        // If no privacy settings found, return default "Nobody" (most restrictive)
        if (privacyValueDataList == null || privacyValueDataList.Count == 0)
        {
            privacyValueDataList = new List<PrivacyValueData>
            {
                new PrivacyValueData(PrivacyValueType.DisallowAll, null)
            };
        }
        
        return new TVector<IPrivacyRule>(privacyValueDataList.Select(p => PrivacyConverter.ToPrivacyRule(p)).ToList());
    }

    public async Task<GlobalPrivacySettings?> GetGlobalPrivacySettingsAsync()
    {
        // Note: This method doesn't have userId context, using 0 as placeholder
        // Consider refactoring to require userId parameter
        var domainSettings = await queryProcessor.ProcessAsync(new MyTelegram.Queries.Privacy.GetGlobalPrivacySettingsQuery(0));
        return domainSettings != null ? ConvertToMessengerSettings(domainSettings) : null;
    }

    public async Task<GlobalPrivacySettingsCacheItem?> GetGlobalPrivacySettingsAsync(long userId)
    {
        var settings = await GetGlobalPrivacySettings(userId);
        if (settings == null)
        {
            return null;
        }
        return new GlobalPrivacySettingsCacheItem(
            settings.ArchiveAndMuteNewNoncontactPeers,
            settings.KeepArchivedUnmuted,
            settings.KeepArchivedFolders,
            settings.HideReadMarks,
            false, // NewNoncontactPeersRequirePremium - default
            false, // DisplayGiftsButton - default
            null   // NoncontactPeersPaidStars - default
        );
    }

    public PrivacyValueData GetPrivacyValueData(IInputPrivacyRule rule)
    {
        return PrivacyConverter.ToPrivacyValueData(rule);
    }

    public List<PrivacyValueData> GetPrivacyValueDataList(IList<IInputPrivacyRule> rules)
    {
        return rules.Select(PrivacyConverter.ToPrivacyValueData).ToList();
    }

    public Task SetPrivacyAsync(IRequestInput requestInput, long selfUserId, IInputPrivacyKey key, IReadOnlyList<IInputPrivacyRule> rules)
    {
        var privacyType = PrivacyConverter.ToPrivacyType(key);
        var privacyValueDataList = rules.Select(PrivacyConverter.ToPrivacyValueData).ToList();
        
        logger.LogInformation("SetPrivacyAsync: UserId={UserId}, PrivacyType={PrivacyType}, ReqMsgId={ReqMsgId}, RequestId={RequestId}", 
            selfUserId, privacyType, requestInput.ReqMsgId, requestInput.RequestId);
        
        // Use full RequestInfo from IRequestInput to preserve ReqMsgId for command deduplication
        var command = new SetPrivacyCommand(UserId.Create(selfUserId), new MyTelegram.RequestInfo(
            requestInput.ReqMsgId,
            selfUserId,
            requestInput.AccessHashKeyId,
            requestInput.AuthKeyId,
            requestInput.PermAuthKeyId,
            requestInput.RequestId,
            requestInput.Layer,
            requestInput.Date,
            requestInput.DeviceType
        ), privacyType, privacyValueDataList);
        
        return commandBus.PublishAsync(command, CancellationToken.None);
    }

    public async Task ApplyPrivacyAsync(long selfUserId, long targetUserId, Action<PrivacyValueType> executeOnPrivacyNotMatch, PrivacyType privacyType)
    {
        // Get privacy settings for the target user (the user whose data is being accessed)
        var privacyReadModels = await queryProcessor.ProcessAsync(new GetPrivacyByUserIdQuery(targetUserId, PrivacyConverter.ToInputPrivacyKey(privacyType)));
        
        if (privacyReadModels == null || privacyReadModels.Count == 0)
        {
            // No privacy settings found - default to most restrictive (Nobody/DisallowAll)
            logger.LogInformation("No privacy settings found for user {TargetUserId}, privacy type {PrivacyType}. Blocking access.", targetUserId, privacyType);
            executeOnPrivacyNotMatch(PrivacyValueType.DisallowAll);
            return;
        }

        var privacyValueDataList = privacyReadModels.First().PrivacyValueDataList;
        
        // Check if access is allowed based on privacy rules
        bool isAllowed = await CheckPrivacyAccessAsync(selfUserId, targetUserId, privacyValueDataList);
        
        if (!isAllowed)
        {
            logger.LogInformation("Privacy check failed for user {SelfUserId} accessing {TargetUserId}, privacy type {PrivacyType}", selfUserId, targetUserId, privacyType);
            // Find the blocking rule type
            var blockingRule = privacyValueDataList.FirstOrDefault(p => p.PrivacyValueType == PrivacyValueType.DisallowAll) 
                ?? privacyValueDataList.FirstOrDefault(p => p.PrivacyValueType.ToString().StartsWith("Disallow"));
            executeOnPrivacyNotMatch(blockingRule?.PrivacyValueType ?? PrivacyValueType.DisallowAll);
        }
    }

    public async Task ApplyPrivacyAsync(long selfUserId, long targetUserId, Action<PrivacyValueType> executeOnPrivacyNotMatch, List<PrivacyType> privacyTypes)
    {
        foreach (var privacyType in privacyTypes)
        {
            await ApplyPrivacyAsync(selfUserId, targetUserId, executeOnPrivacyNotMatch, privacyType);
        }
    }
    
    private async Task<bool> CheckPrivacyAccessAsync(long selfUserId, long targetUserId, IReadOnlyList<PrivacyValueData> privacyRules)
    {
        // If checking own privacy, always allow
        if (selfUserId == targetUserId)
        {
            return true;
        }

        // Default to deny if no rules
        if (privacyRules == null || privacyRules.Count == 0)
        {
            return false;
        }

        bool hasAllowRule = false;
        bool hasDisallowRule = false;
        
        // Check if explicitly disallowed first (disallow rules take precedence)
        foreach (var rule in privacyRules)
        {
            switch (rule.PrivacyValueType)
            {
                case PrivacyValueType.DisallowAll:
                    logger.LogInformation("DisallowAll rule found - blocking all access");
                    return false;
                    
                case PrivacyValueType.DisallowUsers:
                    var disallowedUsers = System.Text.Json.JsonSerializer.Deserialize<List<long>>(rule.JsonData ?? "[]") ?? new List<long>();
                    if (disallowedUsers.Contains(selfUserId))
                    {
                        logger.LogInformation("User {SelfUserId} is in disallow list", selfUserId);
                        return false;
                    }
                    hasDisallowRule = true;
                    break;
                    
                case PrivacyValueType.DisallowContacts:
                    // Check if users are contacts
                    var contact = await queryProcessor.ProcessAsync(new GetContactQuery(targetUserId, selfUserId));
                    if (contact != null)
                    {
                        logger.LogInformation("User {SelfUserId} is a contact and contacts are disallowed", selfUserId);
                        return false;
                    }
                    hasDisallowRule = true;
                    break;
            }
        }

        // Check if explicitly allowed
        foreach (var rule in privacyRules)
        {
            switch (rule.PrivacyValueType)
            {
                case PrivacyValueType.AllowAll:
                    logger.LogInformation("AllowAll rule found - allowing access");
                    return true;
                    
                case PrivacyValueType.AllowUsers:
                    var allowedUsers = System.Text.Json.JsonSerializer.Deserialize<List<long>>(rule.JsonData ?? "[]") ?? new List<long>();
                    if (allowedUsers.Contains(selfUserId))
                    {
                        logger.LogInformation("User {SelfUserId} is in allow list", selfUserId);
                        return true;
                    }
                    hasAllowRule = true;
                    break;
                    
                case PrivacyValueType.AllowContacts:
                    // Check if users are contacts
                    var contact = await queryProcessor.ProcessAsync(new GetContactQuery(targetUserId, selfUserId));
                    if (contact != null)
                    {
                        logger.LogInformation("User {SelfUserId} is a contact and contacts are allowed", selfUserId);
                        return true;
                    }
                    hasAllowRule = true;
                    break;
                    
                case PrivacyValueType.AllowPremium:
                    // Check if user has premium
                    // For now, we'll skip premium check - can be implemented later
                    hasAllowRule = true;
                    break;
            }
        }

        // If there are allow rules but user didn't match any, deny
        // If there are only disallow rules and user didn't match any, allow
        if (hasAllowRule && !hasDisallowRule)
        {
            logger.LogInformation("Has allow rules but user {SelfUserId} didn't match - denying", selfUserId);
            return false;
        }
        
        if (hasDisallowRule && !hasAllowRule)
        {
            logger.LogInformation("Has disallow rules but user {SelfUserId} didn't match - allowing", selfUserId);
            return true;
        }

        // Default: if mixed rules and no match, deny for safety
        logger.LogInformation("No matching rules found for user {SelfUserId} - denying by default", selfUserId);
        return false;
    }

    public async Task ApplyPrivacyListAsync(long selfUserId, IReadOnlyList<long> targetUserIds, Action<PrivacyValueType, long> executeOnPrivacyNotMatch, List<PrivacyType> privacyTypes)
    {
        foreach (var targetUserId in targetUserIds)
        {
            foreach (var privacyType in privacyTypes)
            {
                await ApplyPrivacyAsync(selfUserId, targetUserId, value => executeOnPrivacyNotMatch(value, targetUserId), privacyType);
            }
        }
    }

    public Task SetGlobalPrivacySettingsAsync(long userId, GlobalPrivacySettings globalPrivacySettings)
    {
        var domainGlobalPrivacySettings = new MyTelegram.GlobalPrivacySettings(
            globalPrivacySettings.ArchiveAndMuteNewNoncontactPeers,
            globalPrivacySettings.KeepArchivedUnmuted,
            globalPrivacySettings.KeepArchivedFolders,
            globalPrivacySettings.HideReadTime,
            false, // NewNoncontactPeersRequirePremium - default
            null, // NoncontactPeersPaidStars - default
            false // DisplayGiftsButton - default
        );
        
        var command = new UpdateUserGlobalPrivacySettingsCommand(UserId.Create(userId), new MyTelegram.RequestInfo(
            0, // reqMsgId
            userId,
            0, // accessHashKeyId
            0, // authKeyId
            0, // permAuthKeyId
            Guid.Empty, // requestId
            0, // layer
            0, // date
            MyTelegram.DeviceType.Unknown // deviceType
        ), domainGlobalPrivacySettings);
        return commandBus.PublishAsync(command, CancellationToken.None);
    }

    private async Task<DomainGlobalPrivacySettings?> GetGlobalPrivacySettings(long userId)
    {
        return await queryProcessor.ProcessAsync(new MyTelegram.Queries.Privacy.GetGlobalPrivacySettingsQuery(userId));
    }

    private GlobalPrivacySettings ConvertToMessengerSettings(DomainGlobalPrivacySettings domain)
    {
        return new GlobalPrivacySettings
        {
            ArchiveAndMuteNewNoncontactPeers = domain.ArchiveAndMuteNewNoncontactPeers,
            KeepArchivedUnmuted = domain.KeepArchivedUnmuted,
            KeepArchivedFolders = domain.KeepArchivedFolders,
            HideReadTime = domain.HideReadMarks
        };
    }

    private GlobalPrivacySettingsCacheItem GetPrivacySettingsCacheItem()
    {
        var globalPrivacySettings = GetGlobalPrivacySettings(0).GetAwaiter().GetResult();
        if (globalPrivacySettings == null)
        {
            return new GlobalPrivacySettingsCacheItem(false, false, false, false, false, false, null);
        }
        return new GlobalPrivacySettingsCacheItem(
            globalPrivacySettings.ArchiveAndMuteNewNoncontactPeers,
            globalPrivacySettings.KeepArchivedUnmuted,
            globalPrivacySettings.KeepArchivedFolders,
            globalPrivacySettings.HideReadMarks,
            false, // NewNoncontactPeersRequirePremium - default
            false, // DisplayGiftsButton - default
            null   // NoncontactPeersPaidStars - default
        );
    }
}
