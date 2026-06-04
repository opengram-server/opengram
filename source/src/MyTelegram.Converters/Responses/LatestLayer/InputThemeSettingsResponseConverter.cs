namespace MyTelegram.Converters.Responses.LatestLayer;

internal sealed class InputThemeSettingsResponseConverter : IInputThemeSettingsResponseConverter, ITransientDependency
{
    public int Layer => Layers.LayerLatest;

    public IInputThemeSettings ToLayeredData(TInputThemeSettings obj)
    {
        return obj;
    }
}