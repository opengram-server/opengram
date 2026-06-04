using EventFlow.Aggregates;
using EventFlow.Sagas;
using MyTelegram.Domain.Aggregates.ChatImport;
using MyTelegram.Domain.Events.ChatImport;
using MyTelegram.Domain.Sagas.Identities;

namespace MyTelegram.Domain.Sagas;

public class ChatImportSagaLocator : ISagaLocator
{
    public Task<ISagaId> LocateSagaAsync(IDomainEvent domainEvent, CancellationToken cancellationToken)
    {
        var aggregateEvent = domainEvent as IDomainEvent<ChatImportAggregate, ChatImportId, ChatImportStartedEvent>;
        if (aggregateEvent != null)
        {
            // Use the same ID as the aggregate for simplicity, or derive it
            return Task.FromResult<ISagaId>(new ChatImportSagaId(aggregateEvent.AggregateIdentity.Value));
        }

        return Task.FromResult<ISagaId>(null!);
    }
}
