namespace MyTelegram.Messenger.Converters.ConverterServices.Channels;

public interface IJoinChannelConverterService
{
    MyTelegram.Schema.IUpdates ToJoinChannelUpdates(long selfUserId, SendOutboxMessageCompletedSagaEvent data, int layer);
    MyTelegram.Schema.IUpdates ToJoinChannelUpdates(SendOutboxMessageCompletedSagaEvent data,
        int layer);
}