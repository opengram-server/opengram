namespace MyTelegram.Messenger.Converters.ConverterServices.Messages;

public interface IReadHistoryConverterService
{
    IUpdates ToReadHistoryUpdates(ReadHistoryCompletedSagaEvent aggregateEvent);
    IUpdates ToReadHistoryUpdates(UpdateOutboxMaxIdCompletedSagaEvent eventData);
}