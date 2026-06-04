namespace MyTelegram.Domain.CommandHandlers.Channel;

public class ToggleSignatureCommandHandler : CommandHandler<ChannelAggregate, ChannelId, ToggleSignatureCommand>
{
    public override Task ExecuteAsync(ChannelAggregate aggregate, ToggleSignatureCommand command, CancellationToken cancellationToken)
    {
        aggregate.ToggleSignature(command.RequestInfo, command.SignatureEnabled, command.ProfilesEnabled);

        return Task.CompletedTask;
    }
}