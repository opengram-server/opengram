namespace MyTelegram.Core;

public record FutureAuthTokenCacheItem(long UserId, string AuthTokenId)
{
    public static string GetCacheKey(string futureAuthTokenId)
    {
        return MyCacheKey.With("future_auth_token", futureAuthTokenId);
    }
}