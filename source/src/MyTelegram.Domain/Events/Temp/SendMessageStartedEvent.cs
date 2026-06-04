namespace MyTelegram.Domain.Events.Temp;

public class SendMessageStartedEvent(RequestInfo requestInfo, List<SendMessageItem> sendMessageItems, bool isSendQuickReplyMessages,
    bool isSendGroupedMessages,
    bool clearDraft) : RequestAggregateEvent2<TempAggregate, TempId>(requestInfo)
{
    public List<SendMessageItem> SendMessageItems { get; } = sendMessageItems;
    public bool IsSendQuickReplyMessages { get; } = isSendQuickReplyMessages;
    public bool IsSendGroupedMessages { get; } = isSendGroupedMessages;
    public bool ClearDraft { get; } = clearDraft;
}