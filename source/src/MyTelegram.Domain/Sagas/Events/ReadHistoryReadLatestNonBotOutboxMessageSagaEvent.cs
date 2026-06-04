namespace MyTelegram.Domain.Sagas.Events;

public class ReadHistoryReadLatestNonBotOutboxMessageSagaEvent(long senderPeerId)
    : AggregateEvent<ReadHistorySaga, ReadHistorySagaId>
{
    public long SenderPeerId { get; } = senderPeerId;
}
