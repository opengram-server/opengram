namespace MyTelegram.Messenger.Converters.ConverterServices.Messages;

public interface ISendMessageConverterService
{
    IUpdates ToSelfOtherDeviceUpdates(SendOutboxMessageCompletedSagaEvent aggregateEvent, int layer);

    IUpdates ToUpdates(SendOutboxMessageCompletedSagaEvent aggregateEvent, int layer = 0);
    IUpdates ToUpdates(ReceiveInboxMessageCompletedSagaEvent data);
}