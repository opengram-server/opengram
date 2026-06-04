namespace MyTelegram.Domain.CommandHandlers.UserName;

public class SetUserNameCommandHandler : CommandHandler<UserNameAggregate, UserNameId, SetUserNameCommand>
{
    public override Task ExecuteAsync(UserNameAggregate aggregate,
        SetUserNameCommand command,
        CancellationToken cancellationToken)
    {
        aggregate.UpdateUserName(command.RequestInfo,
            command.Peer,
            command.UserName,
            command.OldUserName
            );
        return Task.CompletedTask;
    }
}