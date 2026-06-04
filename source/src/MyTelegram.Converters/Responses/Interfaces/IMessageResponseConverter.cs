namespace MyTelegram.Converters.Responses.Interfaces;

public interface IMessageResponseConverter
    : IResponseConverter<
        TMessage,
        ILayeredMessage
    >
{
}