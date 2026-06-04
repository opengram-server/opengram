namespace MyTelegram.Domain.CommandHandlers.User;

public class UpdateFirstNameCommandHandler : CommandHandler<UserAggregate, UserId, UpdateFirstNameCommand>
{
    public override Task ExecuteAsync(UserAggregate aggregate, UpdateFirstNameCommand command, CancellationToken cancellationToken)
    {
        aggregate.UpdateFirstName(command.FirstName);

        return Task.CompletedTask;
    }
}