namespace MyTelegram.Domain.CommandHandlers.User;

public class UpdateBotVerifierSettingsCommandHandler : CommandHandler<UserAggregate, UserId, UpdateBotVerifierSettingsCommand>
{
    public override Task ExecuteAsync(UserAggregate aggregate,
        UpdateBotVerifierSettingsCommand command,
        CancellationToken cancellationToken)
    {
        aggregate.UpdateBotVerifierSettings(
            command.BotUserId,
            command.IconEmojiId,
            command.CompanyName,
            command.CanModifyCustomDescription);
        
        return Task.CompletedTask;
    }
}
