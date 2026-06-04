namespace MyTelegram.Domain.Events.User;

public class UserAccountUnfrozenEvent : RequestAggregateEvent2<UserAggregate, UserId>
{
    public long UserId { get; }
    public UnfreezeReason Reason { get; }
    public long? ModeratorUserId { get; }
    public string? Note { get; }

    public UserAccountUnfrozenEvent(
        RequestInfo requestInfo,
        long userId,
        UnfreezeReason reason,
        long? moderatorUserId,
        string? note) : base(requestInfo)
    {
        UserId = userId;
        Reason = reason;
        ModeratorUserId = moderatorUserId;
        Note = note;
    }
}
