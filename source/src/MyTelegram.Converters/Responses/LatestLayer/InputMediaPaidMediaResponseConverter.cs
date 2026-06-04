namespace MyTelegram.Converters.Responses.LatestLayer;

internal sealed class InputMediaPaidMediaResponseConverter : IInputMediaPaidMediaResponseConverter, ITransientDependency
{
    public int Layer => Layers.LayerLatest;

    public IInputMedia ToLayeredData(TInputMediaPaidMedia obj)
    {
        return obj;
    }
}