namespace MyTelegram.Domain.Commands.User;

public class CheckUserStatusCommand(UserId aggregateId, RequestInfo requestInfo)
    : RequestCommand2<UserAggregate, UserId, IExecutionResult>(aggregateId, requestInfo);