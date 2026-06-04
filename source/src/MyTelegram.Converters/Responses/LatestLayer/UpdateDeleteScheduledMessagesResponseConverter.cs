namespace MyTelegram.Converters.Responses.LatestLayer;

internal sealed class UpdateDeleteScheduledMessagesResponseConverter : IUpdateDeleteScheduledMessagesResponseConverter,
    ITransientDependency
{
    public int Layer => Layers.LayerLatest;

    public IUpdate ToLayeredData(TUpdateDeleteScheduledMessages obj)
    {
        return obj;
    }
}