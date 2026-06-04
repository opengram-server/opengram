namespace MyTelegram.Domain.Commands.Channel;

public class SetChannelPtsCommand(
    ChannelId aggregateId,
    long senderPeerId,
    int pts,
    int messageId,
    int date)
    : Command<ChannelAggregate, ChannelId, IExecutionResult>(aggregateId)
{
    public long SenderPeerId { get; } = senderPeerId;
    public int Pts { get; } = pts;
    public int MessageId { get; } = messageId;
    public int Date { get; } = date;
}
