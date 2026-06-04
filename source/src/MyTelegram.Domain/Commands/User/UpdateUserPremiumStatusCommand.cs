namespace MyTelegram.Domain.Commands.User;

public class UpdateUserPremiumStatusCommand(UserId aggregateId, bool premium)
    : Command<UserAggregate, UserId, IExecutionResult>(aggregateId)
{
    public bool Premium { get; } = premium;
}