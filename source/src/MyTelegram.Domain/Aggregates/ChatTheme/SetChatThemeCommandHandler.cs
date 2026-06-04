namespace MyTelegram.Domain.Aggregates.ChatTheme;

public class SetChatThemeCommandHandler : CommandHandler<ChatThemeAggregate, ChatThemeId, SetChatThemeCommand>
{
    public override Task ExecuteAsync(ChatThemeAggregate aggregate, SetChatThemeCommand command,
        CancellationToken cancellationToken)
    {
        aggregate.SetTheme(
            command.RequestInfo,
            command.OwnerPeerId,
            PeerType.User, // Default to User, can be adjusted based on logic
            command.PeerId,
            command.Emoticon,
            command.MessageId
        );
        return Task.CompletedTask;
    }
}
