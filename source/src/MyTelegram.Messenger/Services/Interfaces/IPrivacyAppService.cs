using MyTelegram.Messenger.Services.Impl;

namespace MyTelegram.Messenger.Services.Interfaces;

public interface IPrivacyAppService
{
    Task<IReadOnlyCollection<IPrivacyReadModel>> GetPrivacyListAsync(IReadOnlyList<long> userIds);
    Task<IReadOnlyCollection<IPrivacyReadModel>> GetPrivacyListAsync(long userId);

    Task<TVector<IPrivacyRule>> GetPrivacyRulesAsync(long selfUserId,
        IInputPrivacyKey key);

    Task SetPrivacyAsync(IRequestInput requestInput,
        long selfUserId,
        IInputPrivacyKey key,
        IReadOnlyList<IInputPrivacyRule> ruleList);
    Task ApplyPrivacyAsync(long selfUserId, long targetUserId, Action<PrivacyValueType> executeOnPrivacyNotMatch,
        PrivacyType privacyType);

    //Task ApplyPrivacyAsync(SimpleUserItem userItem, long targetUserId, Action executeOnPrivacyNotMatch,
    //    List<PrivacyType> privacyTypes);
    Task ApplyPrivacyAsync(long selfUserId, long targetUserId, Action<PrivacyValueType> executeOnPrivacyNotMatch,
        List<PrivacyType> privacyTypes);
    //Task ApplyPrivacyListAsync(SimpleUserItem userItem, IReadOnlyList<long> targetUserIdList, Action<long> executeOnPrivacyNotMatch,
    //    List<PrivacyType> privacyTypes);
    Task ApplyPrivacyListAsync(long selfUserId, IReadOnlyList<long> targetUserIdList, Action<PrivacyValueType, long> executeOnPrivacyNotMatch,
        List<PrivacyType> privacyTypes);

    Task SetGlobalPrivacySettingsAsync(long userId, MyTelegram.Messenger.Services.Impl.GlobalPrivacySettings globalPrivacySettings);
    Task<GlobalPrivacySettingsCacheItem?> GetGlobalPrivacySettingsAsync(long userId);
    PrivacyValueData GetPrivacyValueData(IInputPrivacyRule rule);
    List<PrivacyValueData> GetPrivacyValueDataList(IList<IInputPrivacyRule> rules);
}