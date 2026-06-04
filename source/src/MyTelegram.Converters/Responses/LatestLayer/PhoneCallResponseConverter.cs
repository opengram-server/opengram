namespace MyTelegram.Converters.Responses.LatestLayer;

internal sealed class PhoneCallResponseConverter : IPhoneCallResponseConverter, ITransientDependency
{
    public int Layer => Layers.LayerLatest;

    public IPhoneCall ToLayeredData(TPhoneCall obj)
    {
        return obj;
    }
}