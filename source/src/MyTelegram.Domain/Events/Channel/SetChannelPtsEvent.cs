namespace MyTelegram.Domain.Events.Channel;

public class SetChannelPtsEvent(long senderPeerId, int pts, int messageId, int date)
    : AggregateEvent<ChannelAggregate, ChannelId>
{
    public long SenderPeerId { get; } = senderPeerId;
    public int Pts { get; } = pts;
    public int MessageId { get; } = messageId;
    public int Date { get; } = date;
}
