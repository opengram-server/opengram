namespace MyTelegram.Domain.CommandHandlers.User;

public class UpdateAboutCommandHandler : CommandHandler<UserAggregate, UserId, UpdateAboutCommand>
{
    public override Task ExecuteAsync(UserAggregate aggregate, UpdateAboutCommand command, CancellationToken cancellationToken)
    {
        aggregate.UpdateAbout(command.About);

        return Task.CompletedTask;
    }
}