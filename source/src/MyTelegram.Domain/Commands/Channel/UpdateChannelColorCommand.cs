namespace MyTelegram.Domain.Commands.Channel;

public class UpdateChannelColorCommand(
    ChannelId aggregateId,
    RequestInfo requestInfo,
    PeerColor color,
    long? backgroundEmojiId,
    bool forProfile)
    : RequestCommand2<ChannelAggregate, ChannelId, IExecutionResult>(aggregateId, requestInfo)
{
    public PeerColor Color { get; } = color;
    public long? BackgroundEmojiId { get; } = backgroundEmojiId;
    public bool ForProfile { get; } = forProfile;
}