namespace MyTelegram.Converters.Responses.LatestLayer;

internal sealed class ThemeResponseConverter : IThemeResponseConverter, ITransientDependency
{
    public int Layer => Layers.LayerLatest;

    public ITheme ToLayeredData(TTheme obj)
    {
        return obj;
    }
}