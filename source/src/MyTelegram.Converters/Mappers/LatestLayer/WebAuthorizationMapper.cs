namespace MyTelegram.Converters.Mappers.LatestLayer;

internal sealed class WebAuthorizationMapper
    : IObjectMapper<IDeviceReadModel, TWebAuthorization>,
        ILayeredMapper,
        ITransientDependency
{
    public int Layer => Layers.LayerLatest;
    

    public TWebAuthorization Map(IDeviceReadModel source)
    {
        return Map(source, new TWebAuthorization());
    }

    public TWebAuthorization Map(
        IDeviceReadModel source,
        TWebAuthorization destination
    )
    {
        destination.Hash = source.Hash;
        //destination.BotId = source.BotId;
        //destination.Domain = source.Domain;
        destination.Browser = source.AppName;
        destination.Platform = source.Platform;
        destination.DateCreated = source.DateCreated;
        destination.DateActive = source.DateActive;
        destination.Ip = source.Ip;
        //destination.Region = source.Region;

        destination.Region = "Test region";
        destination.Domain = "Test domain";

        return destination;
    }
}