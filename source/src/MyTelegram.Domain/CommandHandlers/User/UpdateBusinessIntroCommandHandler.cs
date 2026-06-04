namespace MyTelegram.Domain.CommandHandlers.User;

public class UpdateBusinessIntroCommandHandler : CommandHandler<UserAggregate, UserId, UpdateBusinessIntroCommand>
{
    public override Task ExecuteAsync(UserAggregate aggregate, UpdateBusinessIntroCommand command, CancellationToken cancellationToken)
    {
        aggregate.UpdateBusinessIntro(command.BusinessIntro);

        return Task.CompletedTask;
    }
}
