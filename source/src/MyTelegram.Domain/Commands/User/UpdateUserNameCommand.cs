namespace MyTelegram.Domain.Commands.User;

public class UpdateUserNameCommand(
    UserId aggregateId,
    RequestInfo requestInfo,
    string userName)
    : RequestCommand2<UserAggregate, UserId, IExecutionResult>(aggregateId, requestInfo)
{
    public string UserName { get; } = userName;
}
