namespace MyTelegram.Domain.Commands.Messaging;

public class CreateOutboxMessageCommand(
    MessageId aggregateId,
    RequestInfo requestInfo,
    MessageItem outboxMessageItem,
    List<long>? mentionedUserIds = null,
    List<ReplyToMsgItem>? replyToMsgItems = null,
    bool clearDraft = true,
    int groupItemCount = 1,
    long? linkedChannelId = null,
    List<long>? chatMembers = null)
    : RequestCommand2<MessageAggregate, MessageId, IExecutionResult>(aggregateId, requestInfo)
{
    public MessageItem OutboxMessageItem { get; } = outboxMessageItem;
    public List<long>? MentionedUserIds { get; } = mentionedUserIds;

    public List<ReplyToMsgItem>? ReplyToMsgItems { get; } = replyToMsgItems;

    public bool ClearDraft { get; } = clearDraft;
    public int GroupItemCount { get; } = groupItemCount;
    public long? LinkedChannelId { get; } = linkedChannelId;
    public List<long>? ChatMembers { get; } = chatMembers;

    protected override IEnumerable<byte[]> GetSourceIdComponents()
    {
        yield return BitConverter.GetBytes(RequestInfo.ReqMsgId);
        yield return BitConverter.GetBytes(OutboxMessageItem.RandomId);
    }
}