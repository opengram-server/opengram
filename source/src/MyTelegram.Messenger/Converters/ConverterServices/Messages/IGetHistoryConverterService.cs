namespace MyTelegram.Messenger.Converters.ConverterServices.Messages;

public interface IGetHistoryConverterService
{
    IMessages ToMessages(IRequestWithAccessHashKeyId request, GetMessageOutput output, int layer);
}