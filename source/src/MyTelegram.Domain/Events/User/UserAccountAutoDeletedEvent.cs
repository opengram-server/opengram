namespace MyTelegram.Domain.Events.User;

public class UserAccountAutoDeletedEvent : AggregateEvent<UserAggregate, UserId>
{
    public long UserId { get; }
    public int FreezeUntilDate { get; }
    public string Reason { get; }

    public UserAccountAutoDeletedEvent(
        long userId,
        int freezeUntilDate,
        string reason = "Freeze period expired")
    {
        UserId = userId;
        FreezeUntilDate = freezeUntilDate;
        Reason = reason;
    }
}
