namespace MyTelegram.Converters.Mappers.LatestLayer;

internal sealed class ChatAdminRightsMapper
    : IObjectMapper<ChatAdminRights, TChatAdminRights>,
        ILayeredMapper,
        ITransientDependency
{
    public int Layer => Layers.LayerLatest;
    

    public TChatAdminRights Map(ChatAdminRights source)
    {
        return Map(source, new TChatAdminRights());
    }

    public TChatAdminRights Map(
        ChatAdminRights source,
        TChatAdminRights destination
    )
    {
        destination.ChangeInfo = source.ChangeInfo;
        destination.PostMessages = source.PostMessages;
        destination.EditMessages = source.EditMessages;
        destination.DeleteMessages = source.DeleteMessages;
        destination.BanUsers = source.BanUsers;
        destination.InviteUsers = source.InviteUsers;
        destination.PinMessages = source.PinMessages;
        destination.AddAdmins = source.AddAdmins;
        destination.Anonymous = source.Anonymous;
        destination.ManageCall = source.ManageCall;
        destination.Other = source.Other;
        destination.ManageTopics = source.ManageTopics;
        destination.PostStories = source.PostStories;
        destination.EditStories = source.EditStories;
        destination.DeleteStories = source.DeleteStories;

        return destination;
    }
}