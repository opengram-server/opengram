namespace MyTelegram.Domain.Aggregates.ChatTheme;

public class ClearChatThemeCommandHandler : CommandHandler<ChatThemeAggregate, ChatThemeId, ClearChatThemeCommand>
{
    public override Task ExecuteAsync(ChatThemeAggregate aggregate, ClearChatThemeCommand command,
        CancellationToken cancellationToken)
    {
        aggregate.ClearTheme(
            command.RequestInfo,
            command.OwnerPeerId,
            PeerType.User, // Default to User, can be adjusted based on logic
            command.PeerId
        );
        return Task.CompletedTask;
    }
}
