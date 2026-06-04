namespace MyTelegram.Core;

public record GlobalPrivacySettingsCacheItem(
    bool ArchiveAndMuteNewNoncontactPeers,
    bool KeepArchivedUnmuted,
    bool KeepArchivedFolders,
    bool HideReadMarks,
    bool NewNoncontactPeersRequirePremium,
    bool DisplayGiftsButton = false,
    long? NoncontactPeersPaidStars = null)
{
    public static string GetCacheKey(long userId)
    {
        return MyCacheKey.With("global_privacy_settings", $"{userId}");
    }
}