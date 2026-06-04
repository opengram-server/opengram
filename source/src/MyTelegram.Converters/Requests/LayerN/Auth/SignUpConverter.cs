using MyTelegram.Schema.Auth.LayerN;

namespace MyTelegram.Converters.Requests.LayerN.Auth;

internal sealed class SignUpConverter
    : IRequestConverter<
        RequestSignUp,
        Schema.Auth.RequestSignUp
    >, ITransientDependency
{
    public Schema.Auth.RequestSignUp ToLatestLayerData(IRequestInput request, RequestSignUp obj)
    {
        return new Schema.Auth.RequestSignUp
        {
            PhoneNumber = obj.PhoneNumber,
            PhoneCodeHash = obj.PhoneCodeHash,
            FirstName = obj.FirstName,
            LastName = obj.LastName
        };
    }
}