namespace MyTelegram.Converters.Responses.LatestLayer;

internal sealed class PhoneCallDiscardedResponseConverter : IPhoneCallDiscardedResponseConverter, ITransientDependency
{
    public int Layer => Layers.LayerLatest;

    public IPhoneCall ToLayeredData(TPhoneCallDiscarded obj)
    {
        return obj;
    }
}