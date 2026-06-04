namespace MyTelegram.Domain.Commands.User;

public class SetUserDefaultHistoryTTLCommand(
    UserId aggregateId,
    RequestInfo requestInfo,
    int period)
    : RequestCommand2<UserAggregate, UserId, IExecutionResult>(aggregateId, requestInfo)
{
    public int Period { get; } = period;
}
