using MyTelegram.Schema.Auth;

namespace MyTelegram.Converters.Responses.Interfaces.Auth;

public interface ISentCodeTypeFirebaseSmsResponseConverter
    : IResponseConverter<
        TSentCodeTypeFirebaseSms,
        ISentCodeType
    >
{
}