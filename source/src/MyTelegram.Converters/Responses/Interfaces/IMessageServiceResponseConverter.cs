namespace MyTelegram.Converters.Responses.Interfaces;

public interface IMessageServiceResponseConverter
    : IResponseConverter<
        TMessageService,
        ILayeredServiceMessage
    >
{
}