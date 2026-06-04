namespace MyTelegram.Domain.Sagas.Events;

public class SendMessageStartedSagaEvent(RequestInfo requestInfo,
    List<SendMessageItem> sendMessageItems,
    List<long>? chatMembers,
    bool clearDraft,
    bool isSendQuickReplyMessages,
    bool isSendGroupedMessages) : RequestAggregateEvent2<SendMessageSaga, SendMessageSagaId>(requestInfo)
{
    public List<SendMessageItem> SendMessageItems { get; } = sendMessageItems;
    public List<long>? ChatMembers { get; } = chatMembers;
    public bool ClearDraft { get; } = clearDraft;
    public bool IsSendQuickReplyMessages { get; } = isSendQuickReplyMessages;
    public bool IsSendGroupedMessages { get; } = isSendGroupedMessages;
}