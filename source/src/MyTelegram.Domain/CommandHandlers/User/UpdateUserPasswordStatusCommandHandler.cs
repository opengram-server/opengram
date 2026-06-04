namespace MyTelegram.Domain.CommandHandlers.User;

public class UpdateUserPasswordStatusCommandHandler : CommandHandler<UserAggregate, UserId, UpdateUserPasswordStatusCommand>
{
    public override Task ExecuteAsync(UserAggregate aggregate, UpdateUserPasswordStatusCommand command, CancellationToken cancellationToken)
    {
        aggregate.UpdatePasswordStatus(command.HasPassword);
        return Task.CompletedTask;
    }
}
