namespace MyTelegram.Converters.Responses.LatestLayer;

internal sealed class DialogFilterResponseConverter : IDialogFilterResponseConverter, ITransientDependency
{
    public int Layer => Layers.LayerLatest;

    public IDialogFilter ToLayeredData(TDialogFilter obj)
    {
        return obj;
    }
}