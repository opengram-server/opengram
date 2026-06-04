namespace MyTelegram.Converters.Mappers.LatestLayer;

internal sealed class PeerSettingsMapper
    : IObjectMapper<PeerSettings, TPeerSettings>,
        ILayeredMapper,
        ITransientDependency
{
    public int Layer => Layers.LayerLatest;
    

    public TPeerSettings Map(PeerSettings source)
    {
        return Map(source, new TPeerSettings());
    }

    public TPeerSettings Map(
        PeerSettings source,
        TPeerSettings destination
    )
    {
        destination.ReportSpam = source.ReportSpam;
        destination.AddContact = source.AddContact;
        destination.BlockContact = source.BlockContact;
        destination.ShareContact = source.ShareContact;
        destination.NeedContactsException = source.NeedContactsException;
        destination.ReportGeo = source.ReportGeo;
        //destination.Autoarchived = source.Autoarchived;
        //destination.InviteMembers = source.InviteMembers;
        //destination.RequestChatBroadcast = source.RequestChatBroadcast;
        //destination.BusinessBotPaused = source.BusinessBotPaused;
        //destination.BusinessBotCanReply = source.BusinessBotCanReply;
        //destination.GeoDistance = source.GeoDistance;
        //destination.RequestChatTitle = source.RequestChatTitle;
        //destination.RequestChatDate = source.RequestChatDate;
        //destination.BusinessBotId = source.BusinessBotId;
        //destination.BusinessBotManageUrl = source.BusinessBotManageUrl;

        return destination;
    }
}