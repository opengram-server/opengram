namespace MyTelegram.Converters.Responses.LatestLayer;

internal sealed class PhoneCallAcceptedResponseConverter : IPhoneCallAcceptedResponseConverter, ITransientDependency
{
    public int Layer => Layers.LayerLatest;

    public IPhoneCall ToLayeredData(TPhoneCallAccepted obj)
    {
        return obj;
    }
}