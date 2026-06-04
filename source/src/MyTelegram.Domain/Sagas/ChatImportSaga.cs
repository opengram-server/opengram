using MyTelegram.Domain.Aggregates.ChatImport;
using MyTelegram.Domain.Events.ChatImport;
using MyTelegram.Domain.Commands.ChatImport;
using MyTelegram.Domain.Sagas.Identities;
using MyTelegram.Domain.Sagas.States;
using EventFlow.Commands;

namespace MyTelegram.Domain.Sagas;

public class ChatImportSaga : AggregateSaga<ChatImportSaga, ChatImportSagaId, ChatImportSagaLocator>,
    ISagaIsStartedBy<ChatImportAggregate, ChatImportId, ChatImportStartedEvent>
{
    private readonly ChatImportSagaState _state = new();

    public ChatImportSaga(ChatImportSagaId id) : base(id)
    {
        Register(_state);
    }

    public async Task HandleAsync(IDomainEvent<ChatImportAggregate, ChatImportId, ChatImportStartedEvent> domainEvent,
        ISagaContext sagaContext,
        CancellationToken cancellationToken)
    {
        // Simulate importing a single welcome message
        var command = new ImportMessageCommand(domainEvent.AggregateIdentity, 0, 0, "Welcome to the imported chat!", 0);
        Publish(command);
        
        // Complete the saga
        // await CompleteAsync(cancellationToken);
    }
}
