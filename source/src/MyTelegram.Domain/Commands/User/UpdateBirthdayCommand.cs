namespace MyTelegram.Domain.Commands.User;

public class UpdateBirthdayCommand(UserId aggregateId, Birthday? birthday)
    : Command<UserAggregate, UserId, IExecutionResult>(aggregateId)
{
    public Birthday? Birthday { get; } = birthday;
}