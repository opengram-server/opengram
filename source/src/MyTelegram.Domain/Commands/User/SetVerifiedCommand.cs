namespace MyTelegram.Domain.Commands.User;

public class SetVerifiedCommand(
    UserId aggregateId,
    bool verified) : Command<UserAggregate, UserId, IExecutionResult>(aggregateId)
{
    public bool Verified { get; } = verified;
}
