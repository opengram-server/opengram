namespace MyTelegram.Messenger.Handlers.LatestLayer.Impl.Account;

///<summary>
/// Возвращает глобальные настройки приватности пользователя.
/// См. <a href="https://corefork.telegram.org/method/account.getGlobalPrivacySettings" />
///</summary>
internal sealed class GetGlobalPrivacySettingsHandler(IPrivacyAppService privacyAppService, ILogger<GetGlobalPrivacySettingsHandler> logger)
    : RpcResultObjectHandler<MyTelegram.Schema.Account.RequestGetGlobalPrivacySettings,
            MyTelegram.Schema.IGlobalPrivacySettings>,
        Account.IGetGlobalPrivacySettingsHandler
{
    protected override async Task<MyTelegram.Schema.IGlobalPrivacySettings> HandleCoreAsync(IRequestInput input,
        MyTelegram.Schema.Account.RequestGetGlobalPrivacySettings obj)
    {
        logger.LogWarning("*** GetGlobalPrivacySettings called for user {UserId} ***", input.UserId);
        var globalPrivacySettings = await privacyAppService.GetGlobalPrivacySettingsAsync(input.UserId);
        if (globalPrivacySettings == null)
        {
            logger.LogWarning("*** globalPrivacySettings is NULL, returning default with DisplayGiftsButton=true ***");
            return new TGlobalPrivacySettings
            {
                DisplayGiftsButton = true, // по умолчанию кнопка подарков включена
                DisallowedGifts = null // null заставляет клиент использовать display_gifts_button для флага SendHide
            };
        }

        var result = new TGlobalPrivacySettings
        {
            ArchiveAndMuteNewNoncontactPeers = globalPrivacySettings.ArchiveAndMuteNewNoncontactPeers,
            HideReadMarks = globalPrivacySettings.HideReadMarks,
            KeepArchivedFolders = globalPrivacySettings.KeepArchivedFolders,
            KeepArchivedUnmuted = globalPrivacySettings.KeepArchivedUnmuted,
            NewNoncontactPeersRequirePremium = globalPrivacySettings.NewNoncontactPeersRequirePremium,
            DisplayGiftsButton = true, // кнопка подарков всегда включена
            NoncontactPeersPaidStars = globalPrivacySettings.NoncontactPeersPaidStars ?? 0,
            DisallowedGifts = null // null заставляет клиент использовать display_gifts_button для флага SendHide
        };
        
        logger.LogWarning("*** Returning DisplayGiftsButton={DisplayGiftsButton}, DisallowedGifts={DisallowedGifts} ***", 
            result.DisplayGiftsButton, 
            result.DisallowedGifts == null ? "NULL" : "NOT NULL");
        return result;
    }
}
