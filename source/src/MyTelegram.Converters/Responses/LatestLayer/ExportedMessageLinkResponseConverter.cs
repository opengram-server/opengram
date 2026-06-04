namespace MyTelegram.Converters.Responses.LatestLayer;

internal sealed class ExportedMessageLinkResponseConverter : IExportedMessageLinkResponseConverter, ITransientDependency
{
    public int Layer => Layers.LayerLatest;

    public IExportedMessageLink ToLayeredData(TExportedMessageLink obj)
    {
        return obj;
    }
}