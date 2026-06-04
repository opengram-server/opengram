namespace MyTelegram.Domain.Commands.User;

public class UpdateUserPasswordStatusCommand(
    UserId aggregateId,
    RequestInfo requestInfo,
    bool hasPassword) : RequestCommand2<UserAggregate, UserId, IExecutionResult>(aggregateId, requestInfo)
{
    public bool HasPassword { get; } = hasPassword;
}
