namespace MyTelegram.Messenger.Services.Caching;

public class BlockCacheAppService : IBlockCacheAppService, ISingletonDependency
{
    public Task BlockAsync(long userId, long targetPeerId)
    {
        return Task.CompletedTask;
    }

    public Task<bool> IsBlockedAsync(long userId, long targetPeerId)
    {
        return Task.FromResult(false);
    }

    public Task UnblockAsync(long userId, long targetPeerId)
    {
        return Task.CompletedTask;
    }
}