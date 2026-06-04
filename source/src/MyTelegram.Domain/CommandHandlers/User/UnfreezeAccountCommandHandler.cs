namespace MyTelegram.Domain.CommandHandlers.User;

public class UnfreezeAccountCommandHandler : CommandHandler<UserAggregate, UserId, UnfreezeAccountCommand>
{
    public override Task ExecuteAsync(UserAggregate aggregate, UnfreezeAccountCommand command, CancellationToken cancellationToken)
    {
        aggregate.UnfreezeAccount(
            command.RequestInfo,
            command.Reason,
            command.ModeratorUserId,
            command.Note);

        return Task.CompletedTask;
    }
}
