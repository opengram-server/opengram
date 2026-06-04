namespace MyTelegram.Domain.Events.Messaging;

public class InboxMessageEditedEvent(
    RequestInfo requestInfo,
    long inboxOwnerPeerId,
    int messageId,
    string newMessage,
    byte[]? entities,
    int editDate,
    Peer toPeer,
    byte[]? media,
    byte[]? replyMarkup,
    List<ReactionCount>? reactions,
    List<Reaction>? recentReactions)
    : RequestAggregateEvent2<MessageAggregate, MessageId>(requestInfo)
{
    public Peer ToPeer { get; } = toPeer;
    public byte[]? Entities { get; } = entities;
    public int EditDate { get; } = editDate;
    public long InboxOwnerPeerId { get; } = inboxOwnerPeerId;
    public byte[]? Media { get; } = media;
    public byte[]? ReplyMarkup { get; } = replyMarkup;
    public List<ReactionCount>? Reactions { get; } = reactions;
    public List<Reaction>? RecentReactions { get; } = recentReactions;

    public int MessageId { get; } = messageId;
    public string NewMessage { get; } = newMessage;
}