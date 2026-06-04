namespace MyTelegram.EventFlow;

public interface IMyInMemorySnapshotPersistence
{
    Task DeleteSnapshotAsync<TAggregate>(IIdentity identity,
        CancellationToken cancellationToken);

    Task<SnapshotContainer?> GetSnapshotAsync<TAggregate>(
        IIdentity identity,
        CancellationToken cancellationToken);

    Task SetSnapshotAsync<TAggregate>(
        IIdentity identity,
        SnapshotContainer snapshotContainer,
        CancellationToken cancellationToken);
}