namespace MyTelegram.Converters.Mappers.LatestLayer;

internal sealed class ChatBannedRightsMapper
    : IObjectMapper<ChatBannedRights, TChatBannedRights>,
        ILayeredMapper,
        ITransientDependency
{
    public int Layer => Layers.LayerLatest;
    

    public TChatBannedRights Map(ChatBannedRights source)
    {
        return Map(source, new TChatBannedRights());
    }

    public TChatBannedRights Map(
        ChatBannedRights source,
        TChatBannedRights destination
    )
    {
        destination.ViewMessages = source.ViewMessages;
        destination.SendMessages = source.SendMessages;
        destination.SendMedia = source.SendMedia;
        destination.SendStickers = source.SendStickers;
        destination.SendGifs = source.SendGifs;
        destination.SendGames = source.SendGames;
        destination.SendInline = source.SendInline;
        destination.EmbedLinks = source.EmbedLinks;
        destination.SendPolls = source.SendPolls;
        destination.ChangeInfo = source.ChangeInfo;
        destination.InviteUsers = source.InviteUsers;
        destination.PinMessages = source.PinMessages;
        destination.ManageTopics = source.ManageTopics;
        destination.SendPhotos = source.SendPhotos;
        destination.SendVideos = source.SendVideos;
        destination.SendRoundvideos = source.SendRoundVideos;
        destination.SendAudios = source.SendAudios;
        destination.SendVoices = source.SendVoices;
        destination.SendDocs = source.SendDocs;
        destination.SendPlain = source.SendPlain;
        destination.UntilDate = source.UntilDate;

        return destination;
    }
}