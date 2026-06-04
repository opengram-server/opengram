namespace MyTelegram.EventFlow;

public class NullEventPersistence : INullEventPersistence
{

    public class MyInMemoryCommittedDomainEvent : ICommittedDomainEvent
    {
        public string AggregateId { get; init; } = null!;
        public string Data { get; init; } = null!;
        public string Metadata { get; init; } = null!;
        public int AggregateSequenceNumber { get; init; }
    }

    public Task<AllCommittedEventsPage> LoadAllCommittedEvents(GlobalPosition globalPosition, int pageSize, CancellationToken cancellationToken)
    {
        return Task.FromResult(new AllCommittedEventsPage(GlobalPosition.Start, []));
    }

    public Task<IReadOnlyCollection<ICommittedDomainEvent>> CommitEventsAsync(IIdentity id, IReadOnlyCollection<SerializedEvent> serializedEvents, CancellationToken cancellationToken)
    {
        if (serializedEvents.Count == 0)
        {
            return Task.FromResult<IReadOnlyCollection<ICommittedDomainEvent>>([]);
        }

        var eventDataModels = serializedEvents
            .Select((e, i) => new MyInMemoryCommittedDomainEvent
            {
                AggregateId = id.Value,
                AggregateSequenceNumber = e.AggregateSequenceNumber,
                Data = e.SerializedData,
                Metadata = e.SerializedMetadata,
            })
            .OrderBy(x => x.AggregateSequenceNumber)
            .ToList();

        return Task.FromResult<IReadOnlyCollection<ICommittedDomainEvent>>(eventDataModels);
    }

    public Task<IReadOnlyCollection<ICommittedDomainEvent>> LoadCommittedEventsAsync(IIdentity id, int fromEventSequenceNumber, CancellationToken cancellationToken)
    {
        return Task.FromResult<IReadOnlyCollection<ICommittedDomainEvent>>([]);
    }

    public Task<IReadOnlyCollection<ICommittedDomainEvent>> LoadCommittedEventsAsync(IIdentity id, int fromEventSequenceNumber, int toEventSequenceNumber,
        CancellationToken cancellationToken)
    {
        return Task.FromResult<IReadOnlyCollection<ICommittedDomainEvent>>([]);
    }

    public Task DeleteEventsAsync(IIdentity id, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}