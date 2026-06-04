// ReSharper disable All

namespace MyTelegram.Messenger.Converters.Mappers.LatestLayer;

internal sealed class ChatInviteExportedMapper
    : IObjectMapper<ChatInviteCreatedEvent, TChatInviteExported>,
        IObjectMapper<ChatInviteEditedEvent, TChatInviteExported>,

ILayeredMapper,
        ITransientDependency
{
    public int Layer => Layers.LayerLatest;
    public int RequestLayer { get; set; }

    public TChatInviteExported Map(ChatInviteCreatedEvent source)
    {
        return Map(source, new TChatInviteExported());
    }

    public TChatInviteExported Map(
        ChatInviteCreatedEvent source,
        TChatInviteExported destination
    )
    {
        destination.Revoked = false;
        destination.Permanent = source.Permanent;
        destination.RequestNeeded = source.RequestNeeded;
        //destination.Link = source.Link;
        destination.AdminId = source.AdminId;
        destination.Date = source.Date;
        destination.StartDate = source.StartDate;
        destination.ExpireDate = source.ExpireDate;
        destination.UsageLimit = source.UsageLimit;
        destination.Usage = 0;
        //destination.Requested = source.Requested;
        //destination.SubscriptionExpired = source.SubscriptionExpired;
        destination.Title = source.Title;
        //destination.SubscriptionPricing = source.SubscriptionPricing;

        return destination;
    }

    [return: NotNullIfNotNull("source")]
    public TChatInviteExported? Map(ChatInviteEditedEvent source)
    {
        return Map(source, new TChatInviteExported());
    }

    [return: NotNullIfNotNull("source")]
    public TChatInviteExported? Map(ChatInviteEditedEvent source, TChatInviteExported destination)
    {
        destination.Revoked = source.Revoked;
        destination.Permanent = source.Permanent;
        destination.RequestNeeded = source.RequestNeeded;
        //destination.Link = source.Link;
        destination.AdminId = source.AdminId;
        destination.Date = source.Date;
        destination.StartDate = source.StartDate;
        destination.ExpireDate = source.ExpireDate;
        destination.UsageLimit = source.UsageLimit;
        destination.Usage = 0;
        //destination.Requested = source.Requested;
        //destination.SubscriptionExpired = source.SubscriptionExpired;
        destination.Title = source.Title;
        //destination.SubscriptionPricing = source.SubscriptionPricing;

        return destination;
    }
}
