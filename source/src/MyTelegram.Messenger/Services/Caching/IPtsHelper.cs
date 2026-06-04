namespace MyTelegram.Messenger.Services.Caching;

public interface IPtsHelper
{
    int GetCachedPts(long ownerId);
    Task<int> IncrementPtsAsync(long ownerId, int currentPts, int ptsCount = 1, long permAuthKeyId = 0, int newUnreadCount = 0);
    Task<int> IncrementQtsAsync(long ownerId, int currentQts, int qtsCount = 1, long permAuthKeyId = 0);
    Task<PtsCacheItem> GetPtsForUserAsync(long userId);
    Task<PtsCacheItem> GetPtsForAuthKeyIdAsync(long userId, long permAuthKeyId);
    Task<bool> UpdatePtsForAuthKeyIdAsync(long userId, long permAuthKeyId, int pts, bool forceUpdate);
    Task SyncCachedPtsToReadModelAsync(long ownerId);
}
