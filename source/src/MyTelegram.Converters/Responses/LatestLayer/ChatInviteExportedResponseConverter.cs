namespace MyTelegram.Converters.Responses.LatestLayer;

internal sealed class ChatInviteExportedResponseConverter : IChatInviteExportedResponseConverter, ITransientDependency
{
    public int Layer => Layers.LayerLatest;

    public IExportedChatInvite ToLayeredData(TChatInviteExported obj)
    {
        return obj;
    }
}