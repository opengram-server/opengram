using MyTelegram.Schema.Payments;

namespace MyTelegram.Converters.Responses.Interfaces.Payments;

public interface IGiveawayInfoResultsResponseConverter
    : IResponseConverter<
        TGiveawayInfoResults,
        IGiveawayInfo
    >
{
}