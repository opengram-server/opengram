namespace MyTelegram.Domain.Aggregates.Messaging;

public class MessageSnapshot(MessageItem messageItem,
        List<InboxItem> inboxItems,
        int senderMessageId,
        bool pinned,
        int editDate,
        bool edited,
        int pts,
        bool isDeleted)
    : ISnapshot
{
    //bool editHide,
    //EditHide = editHide;

    public int EditDate { get; } = editDate;

    //public bool EditHide { get; }
    public bool Edited { get; } = edited;

    public List<InboxItem> InboxItems { get; } = inboxItems;
    public MessageItem MessageItem { get; } = messageItem;
    public bool Pinned { get; } = pinned;
    public int Pts { get; } = pts;
    public bool IsDeleted { get; } = isDeleted;
    public int SenderMessageId { get; } = senderMessageId;
}
