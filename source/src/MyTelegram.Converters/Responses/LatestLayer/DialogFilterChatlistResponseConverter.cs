namespace MyTelegram.Converters.Responses.LatestLayer;

internal sealed class DialogFilterChatlistResponseConverter : IDialogFilterChatlistResponseConverter,
    ITransientDependency
{
    public int Layer => Layers.LayerLatest;

    public IDialogFilter ToLayeredData(TDialogFilterChatlist obj)
    {
        return obj;
    }
}