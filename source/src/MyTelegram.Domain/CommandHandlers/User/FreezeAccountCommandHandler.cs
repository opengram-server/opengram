namespace MyTelegram.Domain.CommandHandlers.User;

public class FreezeAccountCommandHandler : CommandHandler<UserAggregate, UserId, FreezeAccountCommand>
{
    public override Task ExecuteAsync(UserAggregate aggregate, FreezeAccountCommand command, CancellationToken cancellationToken)
    {
        aggregate.FreezeAccount(
            command.RequestInfo,
            command.FreezeSinceDate,
            command.FreezeUntilDate,
            command.Reason,
            command.AppealUrl,
            command.ModeratorUserId,
            command.Note);

        return Task.CompletedTask;
    }
}
