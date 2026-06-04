namespace MyTelegram.Domain.Events.User;

public class UserAccountFrozenEvent : RequestAggregateEvent2<UserAggregate, UserId>
{
    public long UserId { get; }
    public int FreezeSinceDate { get; }
    public int FreezeUntilDate { get; }
    public FreezeReason Reason { get; }
    public string AppealUrl { get; }
    public long? ModeratorUserId { get; }
    public string? Note { get; }

    public UserAccountFrozenEvent(
        RequestInfo requestInfo,
        long userId,
        int freezeSinceDate,
        int freezeUntilDate,
        FreezeReason reason,
        string appealUrl,
        long? moderatorUserId,
        string? note) : base(requestInfo)
    {
        UserId = userId;
        FreezeSinceDate = freezeSinceDate;
        FreezeUntilDate = freezeUntilDate;
        Reason = reason;
        AppealUrl = appealUrl;
        ModeratorUserId = moderatorUserId;
        Note = note;
    }
}
