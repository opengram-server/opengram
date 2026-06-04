namespace MyTelegram.EventFlow;

public class SnapshotWithInMemoryCacheStore(
    IMyInMemorySnapshotPersistence inMemorySnapshotPersistence,
    ISnapshotSerializer snapshotSerializer,
    ISnapshotPersistence snapshotPersistence,
    ILogger<SnapshotStore> logger)
    : ISnapshotWithInMemoryCacheStore
{
    public async Task<SnapshotContainer?> LoadSnapshotAsync<TAggregate, TIdentity, TSnapshot>(TIdentity identity,
        CancellationToken cancellationToken) where TAggregate : ISnapshotAggregateRoot<TIdentity, TSnapshot>
        where TIdentity : IIdentity
        where TSnapshot : ISnapshot
    {
        var cachedSnapshot =
                await LoadSnapshotFromMemoryAsync<TAggregate, TIdentity, TSnapshot>(identity, cancellationToken)
            ;
        if (cachedSnapshot != null)
        {
            logger.LogTrace("Fetching snapshot for {AggregateType} with ID {Id} from memory",
                typeof(TAggregate).PrettyPrint(),
                identity);
            return cachedSnapshot;
        }

        logger.LogTrace(
            "Fetching snapshot for {AggregateType} with ID {Id}",
            typeof(TAggregate).PrettyPrint(),
            identity);
        var committedSnapshot = await snapshotPersistence.GetSnapshotAsync(
                typeof(TAggregate),
                identity,
                cancellationToken)
            ;
        if (committedSnapshot == null)
        {
            logger.LogTrace(
                "No snapshot found for {AggregateType} with ID {Id}",
                typeof(TAggregate).PrettyPrint(),
                identity);
            return null;
        }

        var snapshotContainer = await snapshotSerializer.DeserializeAsync<TAggregate, TIdentity, TSnapshot>(
                committedSnapshot,
                cancellationToken)
            ;

        return snapshotContainer;
    }

    public async Task StoreSnapshotAsync<TAggregate, TIdentity, TSnapshot>(TIdentity identity,
        SnapshotContainer snapshotContainer,
        CancellationToken cancellationToken) where TAggregate : ISnapshotAggregateRoot<TIdentity, TSnapshot>
        where TIdentity : IIdentity
        where TSnapshot : ISnapshot
    {
        var serializedSnapshot = await snapshotSerializer.SerializeAsync<TAggregate, TIdentity, TSnapshot>(
                snapshotContainer,
                cancellationToken)
            ;

        await snapshotPersistence.SetSnapshotAsync(
                typeof(TAggregate),
                identity,
                serializedSnapshot,
                cancellationToken)
            ;
    }

    public Task<SnapshotContainer?> LoadSnapshotFromMemoryAsync<TAggregate, TIdentity, TSnapshot>(TIdentity identity,
        CancellationToken cancellationToken) where TAggregate : ISnapshotAggregateRoot<TIdentity, TSnapshot>
        where TIdentity : IIdentity
        where TSnapshot : ISnapshot
    {
        return inMemorySnapshotPersistence.GetSnapshotAsync<TAggregate>(identity, cancellationToken);
    }

    public Task StoreInMemorySnapshotAsync<TAggregate, TIdentity, TSnapshot>(TIdentity identity,
        SnapshotContainer snapshotContainer,
        CancellationToken cancellationToken) where TAggregate : ISnapshotAggregateRoot<TIdentity, TSnapshot>
        where TIdentity : IIdentity
        where TSnapshot : ISnapshot
    {
        return inMemorySnapshotPersistence.SetSnapshotAsync<TAggregate>(identity,
            snapshotContainer,
            cancellationToken);
    }
}
