namespace MyTelegram.Domain.Commands.Dialog;

public class UpdateReadInboxMaxIdCommand
    (DialogId aggregateId, RequestInfo requestInfo, int maxId, long senderUserId, int senderMessageId,int unreadCount) :
        RequestCommand2<DialogAggregate, DialogId, IExecutionResult>(aggregateId, requestInfo)
{
    public int MaxId { get; } = maxId;
    public long SenderUserId { get; } = senderUserId;
    public int SenderMessageId { get; } = senderMessageId;
    public int UnreadCount { get; } = unreadCount;
}