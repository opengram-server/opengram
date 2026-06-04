namespace MyTelegram.Converters.Responses.LatestLayer;

internal sealed class PhoneCallRequestedResponseConverter : IPhoneCallRequestedResponseConverter, ITransientDependency
{
    public int Layer => Layers.LayerLatest;

    public IPhoneCall ToLayeredData(TPhoneCallRequested obj)
    {
        return obj;
    }
}