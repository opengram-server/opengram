namespace MyTelegram.Messenger.Services.Caching;

public class CachedReadModelUpdater(IEnumerable<ICachedReadModelManager> managers) : ICachedReadModelUpdater, ISingletonDependency
{
    public async Task UpdateAsync(IReadOnlyCollection<IDomainEvent> domainEvents,
        CancellationToken cancellationToken)
    {
        foreach (var manager in managers)
        {
            await manager.ApplyUpdatesAsync(domainEvents, cancellationToken);
        }
    }
}