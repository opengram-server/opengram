namespace MyTelegram.Domain.Commands.User;

public class SetSupportCommand(
    UserId aggregateId,
    bool support) : Command<UserAggregate, UserId, IExecutionResult>(aggregateId)
{
    public bool Support { get; } = support;
}
