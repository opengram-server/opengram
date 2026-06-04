namespace MyTelegram.Domain.Commands.User;

public class FreezeAccountCommand : RequestCommand2<UserAggregate, UserId, IExecutionResult>
{
    public FreezeAccountCommand(
        UserId aggregateId,
        RequestInfo requestInfo,
        int freezeSinceDate,
        int freezeUntilDate,
        FreezeReason reason,
        string appealUrl,
        long? moderatorUserId,
        string? note) : base(aggregateId, requestInfo)
    {
        FreezeSinceDate = freezeSinceDate;
        FreezeUntilDate = freezeUntilDate;
        Reason = reason;
        AppealUrl = appealUrl;
        ModeratorUserId = moderatorUserId;
        Note = note;
    }

    public int FreezeSinceDate { get; }
    public int FreezeUntilDate { get; }
    public FreezeReason Reason { get; }
    public string AppealUrl { get; }
    public long? ModeratorUserId { get; }
    public string? Note { get; }
}
