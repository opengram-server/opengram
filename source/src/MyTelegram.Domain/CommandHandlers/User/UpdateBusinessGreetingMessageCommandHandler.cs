namespace MyTelegram.Domain.CommandHandlers.User;

public class UpdateBusinessGreetingMessageCommandHandler : CommandHandler<UserAggregate, UserId, UpdateBusinessGreetingMessageCommand>
{
    public override Task ExecuteAsync(UserAggregate aggregate, UpdateBusinessGreetingMessageCommand command, CancellationToken cancellationToken)
    {
        aggregate.UpdateBusinessGreetingMessage(command.GreetingMessage);

        return Task.CompletedTask;
    }
}
