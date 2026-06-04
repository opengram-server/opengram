namespace MyTelegram.Domain.Commands.UserName;

public class SetUserNameCommand(
    UserNameId aggregateId,
    RequestInfo requestInfo,
    Peer peer,
    string? userName,
    string? oldUserName
    )
    : Command<UserNameAggregate, UserNameId, IExecutionResult>(aggregateId), IHasRequestInfo
{
    public Peer Peer { get; } = peer;
    public string? UserName { get; } = userName;
    public string? OldUserName { get; } = oldUserName;
    public RequestInfo RequestInfo { get; } = requestInfo;
}