using MyTelegram.Schema.Account.LayerN;

namespace MyTelegram.Converters.Requests.LayerN.Account;

internal sealed class CreateThemeConverter
    : IRequestConverter<
        RequestCreateTheme,
        Schema.Account.RequestCreateTheme
    >, ITransientDependency
{
    public Schema.Account.RequestCreateTheme ToLatestLayerData(IRequestInput request, RequestCreateTheme obj)
    {
        return new Schema.Account.RequestCreateTheme
        {
            Slug = obj.Slug,
            Title = obj.Title,
            Document = obj.Document,
            Settings = obj.Settings == null ? null : [obj.Settings]
        };
    }
}