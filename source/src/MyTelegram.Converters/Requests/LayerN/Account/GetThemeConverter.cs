using MyTelegram.Schema.Account.LayerN;

namespace MyTelegram.Converters.Requests.LayerN.Account;

internal sealed class GetThemeConverter
    : IRequestConverter<
        RequestGetTheme,
        Schema.Account.RequestGetTheme
    >, ITransientDependency
{
    public Schema.Account.RequestGetTheme ToLatestLayerData(IRequestInput request, RequestGetTheme obj)
    {
        return new Schema.Account.RequestGetTheme
        {
            Format = obj.Format,
            Theme = obj.Theme
            // DocumentId = obj.DocumentId
        };
    }
}