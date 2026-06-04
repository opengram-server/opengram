using MyTelegram.Domain.Events.ChatImport;
using MyTelegram.Domain.Aggregates.ChatImport;

using MyTelegram.Domain.Sagas.Identities;

namespace MyTelegram.Domain.Sagas.States;

public class ChatImportSagaState : AggregateState<ChatImportSaga, ChatImportSagaId, ChatImportSagaState>,
    IApply<ChatImportStartedEvent>
{
    public void Apply(ChatImportStartedEvent aggregateEvent)
    {
        // State update logic
    }
}
