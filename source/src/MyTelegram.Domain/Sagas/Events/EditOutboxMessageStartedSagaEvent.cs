namespace MyTelegram.Domain.Sagas.Events;

public class EditOutboxMessageStartedSagaEvent(RequestInfo requestInfo,
    MessageItem oldMessageItem,
    MessageItem newMessageItem
)
    : RequestAggregateEvent2<EditMessageSaga, EditMessageSagaId>(requestInfo)
{
    public MessageItem OldMessageItem { get; } = oldMessageItem;
    public MessageItem NewMessageItem { get; } = newMessageItem;
}
