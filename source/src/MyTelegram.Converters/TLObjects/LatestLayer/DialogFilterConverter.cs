namespace MyTelegram.Converters.TLObjects.LatestLayer;

internal sealed class DialogFilterConverter(IObjectMapper objectMapper) : IDialogFilterConverter, ITransientDependency
{
    public int Layer => Layers.LayerLatest;

    public IDialogFilter ToDialogFilter(DialogFilter dialogFilter)
    {
        return objectMapper.Map<DialogFilter, TDialogFilter>(dialogFilter);
    }
}