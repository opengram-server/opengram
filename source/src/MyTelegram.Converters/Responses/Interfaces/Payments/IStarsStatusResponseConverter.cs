using MyTelegram.Schema.Payments;

namespace MyTelegram.Converters.Responses.Interfaces.Payments;

public interface IStarsStatusResponseConverter
    : IResponseConverter<
        TStarsStatus,
        IStarsStatus
    >
{
}