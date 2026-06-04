namespace MyTelegram.Domain.Commands.User;

public class UpdateUserColorCommand(
    UserId aggregateId,
    RequestInfo requestInfo,
    PeerColor? color,
    bool forProfile)
    : RequestCommand2<UserAggregate, UserId, IExecutionResult>(aggregateId, requestInfo)
{
    public PeerColor? Color { get; } = color;
    public bool ForProfile { get; } = forProfile;
}