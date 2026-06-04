namespace MyTelegram.Converters.Mappers.LatestLayer;

internal sealed class ChatInviteExportedMapper
    : IObjectMapper<IChatInviteReadModel, TChatInviteExported>,
        ILayeredMapper,
        ITransientDependency
{
    public int Layer => Layers.LayerLatest;
    

    public TChatInviteExported Map(IChatInviteReadModel source)
    {
        return Map(source, new TChatInviteExported());
    }

    public TChatInviteExported Map(
        IChatInviteReadModel source,
        TChatInviteExported destination
    )
    {
        destination.Revoked = source.Revoked;
        destination.Permanent = source.Permanent;
        destination.RequestNeeded = source.RequestNeeded;
        destination.Link = source.Link;
        destination.AdminId = source.AdminId;
        destination.Date = source.Date;
        destination.StartDate = source.StartDate;
        destination.ExpireDate = source.ExpireDate;
        destination.UsageLimit = source.UsageLimit;
        destination.Usage = source.Usage;
        destination.Requested = source.Requested;
        //destination.SubscriptionExpired = source.SubscriptionExpired;
        destination.Title = source.Title;
        //destination.SubscriptionPricing = source.SubscriptionPricing;

        return destination;
    }
}