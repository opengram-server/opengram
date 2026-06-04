namespace MyTelegram.EventFlow;

public abstract class MyInMemoryAggregateSaga<TSaga, TIdentity, TLocator>(TIdentity id, IEventStore eventStore)
    : AggregateSaga<TSaga, TIdentity, TLocator>(id), IInMemoryAggregate, ISagaErrorHandler<TSaga>
    where TSaga : AggregateSaga<TSaga, TIdentity, TLocator>
    where TIdentity : ISagaId
    where TLocator : ISagaLocator
{
    public virtual async Task<bool> HandleAsync(ISagaId sagaId,
        SagaDetails sagaDetails,
        Exception exception,
        CancellationToken cancellationToken)
    {
        await CompleteAsync(cancellationToken);
        return false;
    }

    /// <summary>
    /// Mark the saga as completed and remove this saga from memory
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    protected virtual Task CompleteAsync(CancellationToken cancellationToken = default)
    {
        Complete();
        // Aggregate emit events will not be completed immediately,so delay 1000ms to remove it from memory
        Task.Delay(TimeSpan.FromMilliseconds(1000), cancellationToken).ContinueWith(_ =>
                eventStore.DeleteAggregateAsync<TSaga, TIdentity>(Id, cancellationToken),
            cancellationToken);
        return Task.CompletedTask;
    }
}