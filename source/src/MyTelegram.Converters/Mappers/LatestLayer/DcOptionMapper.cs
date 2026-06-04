namespace MyTelegram.Converters.Mappers.LatestLayer;

internal sealed class DcOptionMapper
    : IObjectMapper<DcOption, TDcOption>,
        ILayeredMapper,
        ITransientDependency
{
    public int Layer => Layers.LayerLatest;
    

    public TDcOption Map(DcOption source)
    {
        return Map(source, new TDcOption());
    }

    public TDcOption Map(
        DcOption source,
        TDcOption destination
    )
    {
        destination.Ipv6 = source.Ipv6;
        destination.MediaOnly = source.MediaOnly;
        destination.TcpoOnly = source.TcpoOnly;
        destination.Cdn = source.Cdn;
        destination.Static = source.Static;
        destination.ThisPortOnly = source.ThisPortOnly;
        destination.Id = source.Id;
        destination.IpAddress = source.IpAddress;
        destination.Port = source.Port;
        destination.Secret = source.Secret;

        return destination;
    }
}