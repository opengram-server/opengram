using MyTelegram.Schema.Account.LayerN;

namespace MyTelegram.Converters.Requests.LayerN.Account;

internal sealed class InstallThemeConverter
    : IRequestConverter<
        RequestInstallTheme,
        Schema.Account.RequestInstallTheme
    >, ITransientDependency
{
    public Schema.Account.RequestInstallTheme ToLatestLayerData(IRequestInput request, RequestInstallTheme obj)
    {
        return new Schema.Account.RequestInstallTheme
        {
            Dark = obj.Dark,
            Format = obj.Format,
            Theme = obj.Theme
        };
    }
}