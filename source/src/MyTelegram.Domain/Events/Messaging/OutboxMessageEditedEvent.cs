namespace MyTelegram.Domain.Events.Messaging;

public class OutboxMessageEditedEvent(
    RequestInfo requestInfo,
    IReadOnlyCollection<InboxItem>? inboxItems,
    MessageItem oldMessageItem,
    int messageId,
    string newMessage,
    int editDate,
    byte[]? entities,
    byte[]? media,
    byte[]? replyMarkup,
    List<ReactionCount>? reactions,
    List<Reaction>? recentReactions)
    : RequestAggregateEvent2<MessageAggregate, MessageId>(requestInfo)
{
    //ChatMembers = chatMembers;

    public IReadOnlyCollection<InboxItem>? InboxItems { get; } = inboxItems;
    public MessageItem OldMessageItem { get; } = oldMessageItem;
    public int MessageId { get; } = messageId;
    public string NewMessage { get; } = newMessage;
    public byte[]? Entities { get; } = entities;
    public byte[]? Media { get; } = media;
    public byte[]? ReplyMarkup { get; } = replyMarkup;
    public List<ReactionCount>? Reactions { get; } = reactions;
    public List<Reaction>? RecentReactions { get; } = recentReactions;

    //public List<long>? ChatMembers { get; }
    public int EditDate { get; } = editDate;
}