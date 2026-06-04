namespace MyTelegram.Domain.Commands.Channel;

public class SetChannelEmojiStickersCommand(
    ChannelId aggregateId,
    RequestInfo requestInfo,
    long? stickerSetId)
    : RequestCommand2<ChannelAggregate, ChannelId, IExecutionResult>(aggregateId, requestInfo)
{
    public long? StickerSetId { get; } = stickerSetId;
}
