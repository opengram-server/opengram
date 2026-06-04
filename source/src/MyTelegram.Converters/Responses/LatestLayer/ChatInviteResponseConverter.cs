namespace MyTelegram.Converters.Responses.LatestLayer;

internal sealed class ChatInviteResponseConverter : IChatInviteResponseConverter, ITransientDependency
{
    public int Layer => Layers.LayerLatest;

    public IChatInvite ToLayeredData(TChatInvite obj)
    {
        return obj;
    }
}