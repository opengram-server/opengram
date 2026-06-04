namespace MyTelegram.Domain.Sagas.Events;

public class SendMessageSagaStartedSagaEvent(
    RequestInfo requestInfo,
    List<SendMessageItem> sendMessageItems,
    List<long>? chatMembers,

    bool clearDraft,
    bool isSendQuickReplyMessage,
    bool isSendGroupedMessage
    )
    : RequestAggregateEvent2<SendMessageSaga, SendMessageSagaId>(requestInfo)
{
    public List<SendMessageItem> SendMessageItems { get; } = sendMessageItems;
    public bool ClearDraft { get; } = clearDraft;
    public bool IsSendQuickReplyMessage { get; } = isSendQuickReplyMessage;
    public bool IsSendGroupedMessage { get; } = isSendGroupedMessage;
    public List<long>? ChatMembers { get; } = chatMembers;
}