namespace MyTelegram.Domain.Events.User;

public class UserFrozenAppealReviewedEvent : RequestAggregateEvent2<UserAggregate, UserId>
{
    public long UserId { get; }
    public string AppealId { get; }
    public AppealStatus Status { get; }
    public long ModeratorUserId { get; }
    public string? ReviewNote { get; }

    public UserFrozenAppealReviewedEvent(
        RequestInfo requestInfo,
        long userId,
        string appealId,
        AppealStatus status,
        long moderatorUserId,
        string? reviewNote) : base(requestInfo)
    {
        UserId = userId;
        AppealId = appealId;
        Status = status;
        ModeratorUserId = moderatorUserId;
        ReviewNote = reviewNote;
    }
}
