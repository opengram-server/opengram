namespace MyTelegram.Domain.CommandHandlers.User;

public class UpdateBusinessWorkHoursCommandHandler : CommandHandler<UserAggregate, UserId, UpdateBusinessWorkHoursCommand>
{
    public override Task ExecuteAsync(UserAggregate aggregate, UpdateBusinessWorkHoursCommand command, CancellationToken cancellationToken)
    {
        aggregate.UpdateBusinessWorkHours(command.WorkHours);

        return Task.CompletedTask;
    }
}
