namespace MyTelegram.Domain.CommandHandlers.Channel;

public class SetChannelCustomVerificationCommandHandler : CommandHandler<ChannelAggregate, ChannelId, SetChannelCustomVerificationCommand>
{
    public override Task ExecuteAsync(ChannelAggregate aggregate,
        SetChannelCustomVerificationCommand command,
        CancellationToken cancellationToken)
    {
        if (command.Enabled)
        {
            var botUserId = command.BotUserId ?? command.RequestInfo.UserId;
            
            // Validation will be done in handler layer before calling this
            aggregate.SetChannelCustomVerification(
                command.RequestInfo,
                command.ChannelId,
                botUserId,
                command.IconEmojiId,
                command.Description,
                command.CustomDescription);
        }
        else
        {
            aggregate.RemoveChannelCustomVerification(command.RequestInfo, command.ChannelId);
        }
        
        return Task.CompletedTask;
    }
}
