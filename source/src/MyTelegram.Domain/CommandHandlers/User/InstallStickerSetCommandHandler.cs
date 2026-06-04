namespace MyTelegram.Domain.CommandHandlers.User;

public class InstallStickerSetCommandHandler : CommandHandler<UserAggregate, UserId, InstallStickerSetCommand>
{
    public override Task ExecuteAsync(UserAggregate aggregate, InstallStickerSetCommand command, CancellationToken cancellationToken)
    {
        aggregate.InstallStickerSet(command.RequestInfo, command.StickerSetId, command.Archived, command.StickerSetType);
        return Task.CompletedTask;
    }
}
