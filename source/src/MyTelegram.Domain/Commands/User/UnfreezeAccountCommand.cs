namespace MyTelegram.Domain.Commands.User;

public class UnfreezeAccountCommand : RequestCommand2<UserAggregate, UserId, IExecutionResult>
{
    public UnfreezeAccountCommand(
        UserId aggregateId,
        RequestInfo requestInfo,
        UnfreezeReason reason,
        long? moderatorUserId,
        string? note) : base(aggregateId, requestInfo)
    {
        Reason = reason;
        ModeratorUserId = moderatorUserId;
        Note = note;
    }

    public UnfreezeReason Reason { get; }
    public long? ModeratorUserId { get; }
    public string? Note { get; }
}
