namespace MyTelegram.Converters.Responses.LatestLayer;

internal sealed class PhoneCallWaitingResponseConverter : IPhoneCallWaitingResponseConverter, ITransientDependency
{
    public int Layer => Layers.LayerLatest;

    public IPhoneCall ToLayeredData(TPhoneCallWaiting obj)
    {
        return obj;
    }
}