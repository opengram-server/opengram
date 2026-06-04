namespace MyTelegram.Converters.Responses.Interfaces;

public interface IPhoneCallWaitingResponseConverter
    : IResponseConverter<
        TPhoneCallWaiting,
        IPhoneCall
    >
{
}