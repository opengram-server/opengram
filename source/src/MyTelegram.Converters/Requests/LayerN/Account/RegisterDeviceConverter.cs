using MyTelegram.Schema.Account.LayerN;

namespace MyTelegram.Converters.Requests.LayerN.Account;

internal sealed class RegisterDeviceConverter
    : IRequestConverter<
        RequestRegisterDevice,
        Schema.Account.RequestRegisterDevice
    >, ITransientDependency
{
    public Schema.Account.RequestRegisterDevice ToLatestLayerData(IRequestInput request, RequestRegisterDevice obj)
    {
        return new Schema.Account.RequestRegisterDevice
        {
            TokenType = obj.TokenType,
            Token = obj.Token,
            OtherUids = []
        };
    }
}