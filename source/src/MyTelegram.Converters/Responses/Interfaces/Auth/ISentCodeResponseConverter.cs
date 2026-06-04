using MyTelegram.Schema.Auth;

namespace MyTelegram.Converters.Responses.Interfaces.Auth;

public interface ISentCodeResponseConverter
    : IResponseConverter<
        TSentCode,
        ISentCode
    >
{
}