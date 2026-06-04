namespace MyTelegram.Converters.Responses.LatestLayer;

internal sealed class GroupCallResponseConverter : IGroupCallResponseConverter, ITransientDependency
{
    public int Layer => Layers.LayerLatest;

    public IGroupCall ToLayeredData(TGroupCall obj)
    {
        return obj;
    }
}