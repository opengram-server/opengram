namespace MyTelegram;

public class InboxItem(
    long inboxOwnerPeerId,
    int inboxMessageId)
{
    public int InboxMessageId { get; init; } = inboxMessageId;

    public long InboxOwnerPeerId { get; init; } = inboxOwnerPeerId;
}