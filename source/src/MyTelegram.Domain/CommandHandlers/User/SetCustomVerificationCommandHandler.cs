namespace MyTelegram.Domain.CommandHandlers.User;

public class SetCustomVerificationCommandHandler : CommandHandler<UserAggregate, UserId, SetCustomVerificationCommand>
{
    public override Task ExecuteAsync(UserAggregate aggregate,
        SetCustomVerificationCommand command,
        CancellationToken cancellationToken)
    {
        if (command.Enabled)
        {
            var botUserId = command.BotUserId ?? command.RequestInfo.UserId;
            
            // Validation will be done in handler layer before calling this
            aggregate.SetCustomVerification(
                command.RequestInfo,
                command.TargetUserId,
                botUserId,
                command.IconEmojiId,
                command.Description,
                command.CustomDescription);
        }
        else
        {
            aggregate.RemoveCustomVerification(command.RequestInfo, command.TargetUserId);
        }
        
        return Task.CompletedTask;
    }
}
