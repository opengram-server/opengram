namespace MyTelegram.Domain.CommandHandlers.Channel;

public class SetChannelEmojiStickersCommandHandler : CommandHandler<ChannelAggregate, ChannelId, SetChannelEmojiStickersCommand>
{
    public override Task ExecuteAsync(ChannelAggregate aggregate, SetChannelEmojiStickersCommand command, CancellationToken cancellationToken)
    {
        aggregate.SetEmojiStickers(command.RequestInfo, command.StickerSetId);
        return Task.CompletedTask;
    }
}
