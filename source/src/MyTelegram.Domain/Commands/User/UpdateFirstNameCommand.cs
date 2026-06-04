namespace MyTelegram.Domain.Commands.User;

public class UpdateFirstNameCommand(UserId aggregateId, string firstName) : Command<UserAggregate, UserId, IExecutionResult>(aggregateId)
{
    public string FirstName { get; } = firstName;
}