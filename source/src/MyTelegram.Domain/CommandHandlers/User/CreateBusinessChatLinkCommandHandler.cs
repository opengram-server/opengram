namespace MyTelegram.Domain.CommandHandlers.User;

public class CreateBusinessChatLinkCommandHandler : CommandHandler<UserAggregate, UserId, CreateBusinessChatLinkCommand>
{
    public override Task ExecuteAsync(UserAggregate aggregate, CreateBusinessChatLinkCommand command, CancellationToken cancellationToken)
    {
        aggregate.CreateBusinessChatLink(command.ChatLink);

        return Task.CompletedTask;
    }
}
