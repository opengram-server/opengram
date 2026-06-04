namespace MyTelegram.Domain.CommandHandlers.User;

public class UpdateBusinessLocationCommandHandler : CommandHandler<UserAggregate, UserId, UpdateBusinessLocationCommand>
{
    public override Task ExecuteAsync(UserAggregate aggregate, UpdateBusinessLocationCommand command, CancellationToken cancellationToken)
    {
        aggregate.UpdateBusinessLocation(command.Location);

        return Task.CompletedTask;
    }
}
