namespace MyTelegram.Domain.CommandHandlers.User;

public class SetUserDefaultHistoryTTLCommandHandler : CommandHandler<UserAggregate, UserId, SetUserDefaultHistoryTTLCommand>
{
    public override Task ExecuteAsync(UserAggregate aggregate, SetUserDefaultHistoryTTLCommand command, CancellationToken cancellationToken)
    {
        aggregate.SetDefaultHistoryTTL(command.RequestInfo, command.Period);
        return Task.CompletedTask;
    }
}
