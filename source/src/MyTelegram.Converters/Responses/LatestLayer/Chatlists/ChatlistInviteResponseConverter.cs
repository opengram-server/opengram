using MyTelegram.Schema.Chatlists;

namespace MyTelegram.Converters.Responses.LatestLayer.Chatlists;

internal sealed class ChatlistInviteResponseConverter : IChatlistInviteResponseConverter, ITransientDependency
{
    public int Layer => Layers.LayerLatest;

    public IChatlistInvite ToLayeredData(TChatlistInvite obj)
    {
        return obj;
    }
}