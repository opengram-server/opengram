namespace MyTelegram.Domain.CommandHandlers.User;

public class UpdateBusinessAwayMessageCommandHandler : CommandHandler<UserAggregate, UserId, UpdateBusinessAwayMessageCommand>
{
    public override Task ExecuteAsync(UserAggregate aggregate, UpdateBusinessAwayMessageCommand command, CancellationToken cancellationToken)
    {
        aggregate.UpdateBusinessAwayMessage(command.AwayMessage);

        return Task.CompletedTask;
    }
}
