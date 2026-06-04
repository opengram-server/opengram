namespace MyTelegram.Domain.Events.Dialog;

public class MentionReadEvent(long ownerUserId, Peer toPeer, int messageId, int unreadMentionsCount)
    : AggregateEvent<DialogAggregate, DialogId>
{
    public long OwnerUserId { get; } = ownerUserId;
    public Peer ToPeer { get; } = toPeer;
    public int MessageId { get; } = messageId;
    public int UnreadMentionsCount { get; } = unreadMentionsCount;
}