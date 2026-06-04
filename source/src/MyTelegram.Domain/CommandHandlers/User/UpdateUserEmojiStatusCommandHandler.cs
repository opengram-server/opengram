namespace MyTelegram.Domain.CommandHandlers.User;

public class UpdateUserEmojiStatusCommandHandler : CommandHandler<UserAggregate, UserId, UpdateUserEmojiStatusCommand>
{
    public override Task ExecuteAsync(UserAggregate aggregate, UpdateUserEmojiStatusCommand command, CancellationToken cancellationToken)
    {
        aggregate.UpdateEmojiStatus(command.RequestInfo, command.EmojiStatusDocumentId, command.EmojiStatusValidUntil, command.EmojiStatusCollectibleId);
        return Task.CompletedTask;
    }
}
