namespace MyTelegram.Messenger.Services.Caching;

public interface IContactCacheAppService
{
    Task<bool> IsExistsAsync(long selfUserId, long targetUserId);
    void Add(long selfUserId, long targetUserId);
    void Remove(long selfUserId, long targetUserId);
}