namespace MyTelegram.Domain.CommandHandlers.User;

public class UpdateBirthdayCommandHandler : CommandHandler<UserAggregate, UserId, UpdateBirthdayCommand>
{
    public override Task ExecuteAsync(UserAggregate aggregate, UpdateBirthdayCommand command, CancellationToken cancellationToken)
    {
        aggregate.UpdateBirthday(command.Birthday);

        return Task.CompletedTask;
    }
}