namespace MyTelegram.Domain.CommandHandlers.User;

public class CreateBotVerifierCommandHandler : CommandHandler<UserAggregate, UserId, CreateBotVerifierCommand>
{
    public override Task ExecuteAsync(UserAggregate aggregate,
        CreateBotVerifierCommand command,
        CancellationToken cancellationToken)
    {
        aggregate.CreateBotVerifier(
            command.BotUserId,
            command.IconEmojiId,
            command.CompanyName,
            command.CanModifyCustomDescription);
        
        return Task.CompletedTask;
    }
}
