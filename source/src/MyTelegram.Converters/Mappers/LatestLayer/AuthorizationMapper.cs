namespace MyTelegram.Converters.Mappers.LatestLayer;

internal sealed class AuthorizationMapper
    : IObjectMapper<IDeviceReadModel, TAuthorization>,
        ILayeredMapper,
        ITransientDependency
{
    public int Layer => Layers.LayerLatest;
    

    public TAuthorization Map(IDeviceReadModel source)
    {
        return Map(source, new TAuthorization());
    }

    public TAuthorization Map(
        IDeviceReadModel source,
        TAuthorization destination
    )
    {
        //destination.Current = source.Current;
        destination.OfficialApp = source.OfficialApp;
        destination.PasswordPending = source.PasswordPending;
        //destination.EncryptedRequestsDisabled = source.EncryptedRequestsDisabled;
        //destination.CallRequestsDisabled = source.CallRequestsDisabled;
        //destination.Unconfirmed = source.Unconfirmed;
        destination.Hash = source.Hash;
        destination.DeviceModel = source.DeviceModel;
        destination.Platform = source.Platform;
        destination.SystemVersion = source.SystemVersion;
        destination.ApiId = source.ApiId;
        destination.AppName = source.AppName;
        destination.AppVersion = source.AppVersion;
        destination.DateCreated = source.DateCreated;
        destination.DateActive = source.DateActive;
        destination.Ip = source.Ip;
        //destination.Country = source.Country;
        //destination.Region = source.Region;

        return destination;
    }
}