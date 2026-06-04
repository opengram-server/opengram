namespace MyTelegram.Domain.Commands.User;

public class ReviewFrozenAppealCommand : RequestCommand2<UserAggregate, UserId, IExecutionResult>
{
    public ReviewFrozenAppealCommand(
        UserId aggregateId,
        RequestInfo requestInfo,
        string appealId,
        AppealStatus status,
        long moderatorUserId,
        string? reviewNote) : base(aggregateId, requestInfo)
    {
        AppealId = appealId;
        Status = status;
        ModeratorUserId = moderatorUserId;
        ReviewNote = reviewNote;
    }

    public string AppealId { get; }
    public AppealStatus Status { get; }
    public long ModeratorUserId { get; }
    public string? ReviewNote { get; }
}
