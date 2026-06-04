namespace MyTelegram.Domain.Sagas.Events;

public class OutboxMessageCreatedSagaEvent(RequestInfo requestInfo,
    MessageItem messageItem,
    List<long>? mentionedUserIds,
    List<ReplyToMsgItem>? replyToMsgItems,
    List<long>? chatMembers) : RequestAggregateEvent2<SendMessageSaga, SendMessageSagaId>(requestInfo)
{
    public MessageItem MessageItem { get; } = messageItem;
    public List<long>? MentionedUserIds { get; } = mentionedUserIds;
    public List<ReplyToMsgItem>? ReplyToMsgItems { get; } = replyToMsgItems;
    public List<long>? ChatMembers { get; } = chatMembers;
}