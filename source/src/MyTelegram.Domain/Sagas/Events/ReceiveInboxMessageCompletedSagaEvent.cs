namespace MyTelegram.Domain.Sagas.Events;

public class ReceiveInboxMessageCompletedSagaEvent(List<MessageItem> messageItems,
    bool isSendQuickReplyMessages,
    bool isSendGroupedMessages
    )
    : AggregateEvent<SendMessageSaga, SendMessageSagaId>
{
    public MessageItem MessageItem => MessageItems.Last();
    public List<MessageItem> MessageItems { get; } = messageItems;

    public bool IsSendQuickReplyMessages { get; } = isSendQuickReplyMessages;

    public bool IsSendGroupedMessages { get; } = isSendGroupedMessages;
}