namespace MyTelegram.Domain.Events.Dialog;

public class DialogUnreadMarkChangedEvent(bool unreadMark) : AggregateEvent<DialogAggregate, DialogId>
{
    public bool UnreadMark { get; } = unreadMark;
}