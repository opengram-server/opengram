namespace MyTelegram.Core;

public record TakeoutCacheItem(
    bool Contacts,
    bool MessageUsers,
    bool MessageChats,
    bool MessageMegagroups,
    bool MessageChannels,
    bool Files,
    long? FileMaxSize
)
{
    public static string GetCacheKey(long permAuthKeyId)
    {
        return MyCacheKey.With("takeout", $"{permAuthKeyId:x2}");
    }
}