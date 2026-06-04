namespace MyTelegram.Domain.CommandHandlers.UserName;

public class DeleteUserNameCommandHandler : CommandHandler<UserNameAggregate, UserNameId, DeleteUserNameCommand>
{
    public override Task ExecuteAsync(UserNameAggregate aggregate,
        DeleteUserNameCommand command,
        CancellationToken cancellationToken)
    {
        aggregate.Delete();
        return Task.CompletedTask;
    }
}
