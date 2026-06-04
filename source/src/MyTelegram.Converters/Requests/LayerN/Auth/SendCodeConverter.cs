using MyTelegram.Schema.Auth.LayerN;

namespace MyTelegram.Converters.Requests.LayerN.Auth;

internal sealed class SendCodeConverter
    : IRequestConverter<
        RequestSendCode,
        Schema.Auth.RequestSendCode
    >, ITransientDependency
{
    public Schema.Auth.RequestSendCode ToLatestLayerData(IRequestInput request, RequestSendCode obj)
    {
        return new Schema.Auth.RequestSendCode
        {
            PhoneNumber = obj.PhoneNumber,
            ApiId = obj.ApiId,
            ApiHash = obj.ApiHash,
            Settings = new TCodeSettings
                { AllowAppHash = obj.AllowFlashcall, CurrentNumber = obj.CurrentNumber ?? false }
        };
    }
}