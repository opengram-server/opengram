namespace MyTelegram.Domain.Sagas.Events;

public class InboxMessageCreatedSagaEvent(RequestInfo requestInfo, MessageItem messageItem) : RequestAggregateEvent2<SendMessageSaga, SendMessageSagaId>(requestInfo)
{
    public MessageItem MessageItem { get; } = messageItem;
}