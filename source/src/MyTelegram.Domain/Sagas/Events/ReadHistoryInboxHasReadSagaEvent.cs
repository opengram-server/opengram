namespace MyTelegram.Domain.Sagas.Events;

public class ReadHistoryInboxHasReadSagaEvent(
    bool isOut,
    bool senderIsBot,
    bool needReadLatestNoneBotOutboxMessage)
    : AggregateEvent<ReadHistorySaga, ReadHistorySagaId>
{
    public bool IsOut { get; } = isOut;
    public bool NeedReadLatestNoneBotOutboxMessage { get; } = needReadLatestNoneBotOutboxMessage;
    public bool SenderIsBot { get; } = senderIsBot;
}
