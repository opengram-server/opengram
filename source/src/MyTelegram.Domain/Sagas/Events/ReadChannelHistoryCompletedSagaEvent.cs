namespace MyTelegram.Domain.Sagas.Events;

public class
    ReadChannelHistoryCompletedSagaEvent(
        RequestInfo requestInfo,
        long channelId,
        long senderPeerId,
        int messageId)
    : RequestAggregateEvent2<ReadChannelHistorySaga, ReadChannelHistorySagaId>(requestInfo)
{
    public long ChannelId { get; } = channelId;
    public int MessageId { get; } = messageId;
    public long SenderPeerId { get; } = senderPeerId;
}
