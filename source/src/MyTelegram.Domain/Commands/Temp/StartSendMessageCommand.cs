namespace MyTelegram.Domain.Commands.Temp;

public class StartSendMessageCommand(
    TempId aggregateId,
    RequestInfo requestInfo, List<SendMessageItem> sendMessageItems, bool clearDraft = true, bool isSendGroupedMessages = false, bool isSendQuickReplyMessages = false, int groupItemCount = 1

) : RequestCommand2<TempAggregate, TempId, IExecutionResult>(aggregateId, requestInfo)
{
    public List<SendMessageItem> SendMessageItems { get; } = sendMessageItems;
    public bool IsSendGroupedMessages { get; } = isSendGroupedMessages;
    public bool IsSendQuickReplyMessages { get; } = isSendQuickReplyMessages;
    public bool ClearDraft { get; } = clearDraft;
    public int GroupItemCount { get; } = groupItemCount;
}