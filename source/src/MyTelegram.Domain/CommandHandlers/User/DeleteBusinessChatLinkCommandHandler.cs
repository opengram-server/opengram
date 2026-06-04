namespace MyTelegram.Domain.CommandHandlers.User;

public class DeleteBusinessChatLinkCommandHandler : CommandHandler<UserAggregate, UserId, DeleteBusinessChatLinkCommand>
{
    public override Task ExecuteAsync(UserAggregate aggregate, DeleteBusinessChatLinkCommand command, CancellationToken cancellationToken)
    {
        aggregate.DeleteBusinessChatLink(command.LinkId);

        return Task.CompletedTask;
    }
}
