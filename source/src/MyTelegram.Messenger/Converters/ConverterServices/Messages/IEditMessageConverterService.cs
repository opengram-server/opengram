namespace MyTelegram.Messenger.Converters.ConverterServices.Messages;

public interface IEditMessageConverterService
{
    IUpdates ToEditQuickReplyMessageUpdates(OutboxMessageEditCompletedSagaEvent data, int layer);
    IUpdates ToEditMessageUpdates(InboxMessageEditCompletedSagaEvent data);
    IUpdates ToEditMessageUpdates(long selfUserId, OutboxMessageEditCompletedSagaEvent data, int layer);
}