namespace MyTelegram.Converters.Responses.LatestLayer;

internal sealed class UpdateGroupCallResponseConverter : IUpdateGroupCallResponseConverter, ITransientDependency
{
    public int Layer => Layers.LayerLatest;

    public IUpdate ToLayeredData(TUpdateGroupCall obj)
    {
        return obj;
    }
}