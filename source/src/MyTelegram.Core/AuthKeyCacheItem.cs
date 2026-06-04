namespace MyTelegram.Core;

public record AuthKeyCacheItem(byte[] AuthKeyData, long ServerSalt, bool IsPermanent, long UserId = 0)
{
    public static string GetCacheKey(long authKey)
    {
        return MyCacheKey.With("authkey", $"{authKey:x2}");
    }
}