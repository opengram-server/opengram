namespace MyTelegram.Domain.CommandHandlers.User;

public class UpdatePersonalChannelCommandHandler : CommandHandler<UserAggregate, UserId, UpdatePersonalChannelCommand>
{
    public override Task ExecuteAsync(UserAggregate aggregate, UpdatePersonalChannelCommand command, CancellationToken cancellationToken)
    {
        aggregate.UpdatePersonalChannel(command.RequestInfo,command.PersonalChannelId);

        return Task.CompletedTask;
    }
}