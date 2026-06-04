namespace MyTelegram.Converters.Responses.Interfaces;

public interface IPhoneCallRequestedResponseConverter
    : IResponseConverter<
        TPhoneCallRequested,
        IPhoneCall
    >
{
}