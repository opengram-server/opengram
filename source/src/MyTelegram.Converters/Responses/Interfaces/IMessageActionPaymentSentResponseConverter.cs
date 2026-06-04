namespace MyTelegram.Converters.Responses.Interfaces;

public interface IMessageActionPaymentSentResponseConverter
    : IResponseConverter<
        TMessageActionPaymentSent,
        IMessageAction
    >
{
}