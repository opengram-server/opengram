namespace MyTelegram.Domain.Sagas.Events;

public class SendOutboxMessageCompletedSagaEvent(RequestInfo requestInfo,
    List<MessageItem> messageItems,
    List<long>? mentionedUserIds,
    bool isSendQuickReplyMessages,
    bool isSendGroupedMessages,
    IReadOnlyCollection<long>? botUserIds) :
    RequestAggregateEvent2<SendMessageSaga, SendMessageSagaId>(requestInfo)
{
    public MessageItem MessageItem => MessageItems.Last();

    public List<MessageItem> MessageItems { get; } = messageItems;
    public List<long>? MentionedUserIds { get; } = mentionedUserIds;
    public bool IsSendQuickReplyMessages { get; } = isSendQuickReplyMessages;
    public bool IsSendGroupedMessages { get; } = isSendGroupedMessages;
    public IReadOnlyCollection<long>? BotUserIds { get; } = botUserIds;
}