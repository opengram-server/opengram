namespace MyTelegram.Domain.Sagas.Events;

public class InboxMessageEditCompletedSagaEvent(
    MessageItem oldMessageItem,
    MessageItem newMessageItem)
    : AggregateEvent<EditMessageSaga, EditMessageSagaId>
{
    public MessageItem OldMessageItem { get; } = oldMessageItem;
    public MessageItem NewMessageItem { get; } = newMessageItem;
}