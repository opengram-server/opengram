namespace MyTelegram.Domain.CommandHandlers.UserName;

public class CreateUserNameCommandHandler : CommandHandler<UserNameAggregate, UserNameId, CreateUserNameCommand>
{
    public override Task ExecuteAsync(UserNameAggregate aggregate,
        CreateUserNameCommand command,
        CancellationToken cancellationToken)
    {
        aggregate.Create(command.Peer, command.UserName, command.Date);
        return Task.CompletedTask;
    }
}